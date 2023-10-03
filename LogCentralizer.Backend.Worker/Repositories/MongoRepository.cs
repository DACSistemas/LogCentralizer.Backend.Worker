using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Providers;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using MongoDB.Driver;

namespace LogCentralizer.BackEnd.Worker.Repositories
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly Dictionary<string, IMongoCollection<TDocument>> mongoCollections = new();
        private readonly object _lock = new();
        public MongoRepository()
        {
            foreach (var citySettings in CitySettingsProvider.AllCitySettings)
            {
                var database = new MongoClient(citySettings.Value.MongoDbSettings.ConnectionString).GetDatabase(citySettings.Value.MongoDbSettings.DatabaseName);
                var _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

                mongoCollections.Add(citySettings.Key, _collection);

            }
        }
        public IMongoCollection<TDocument> GetCollection(string queueName)
        {
            lock (_lock)
            {
                var mongoCollection = mongoCollections[queueName];
                return mongoCollection;
            }
        }
        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }
        public virtual Task InsertOneAsync(TDocument document, string cityName)
        {
            lock (_lock)
            {
                var _collection = mongoCollections[cityName];
                return Task.Run(() => _collection.InsertOneAsync(document));
            }
        }
       
    }
}
