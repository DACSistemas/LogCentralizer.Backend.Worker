using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("_id")]
        ObjectId Id { get; set; }

    }
}
