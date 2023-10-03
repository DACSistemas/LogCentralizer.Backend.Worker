using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using MongoDB.Driver;

namespace LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        IMongoCollection<TDocument> GetCollection(string queueName);
        Task InsertOneAsync(TDocument document, string cityName);
    }
}
