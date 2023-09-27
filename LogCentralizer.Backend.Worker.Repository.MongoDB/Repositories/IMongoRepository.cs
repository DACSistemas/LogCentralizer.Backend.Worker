using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        IMongoCollection<TDocument> GetCollection(string queueName);
        void InsertOne(TDocument document, string cityName);
        Task InsertOneAsync(TDocument document, string cityName);
    }
}
