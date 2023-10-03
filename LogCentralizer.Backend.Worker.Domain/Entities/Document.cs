using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    public abstract class Document : IDocument
    {

        [BsonElement("_id")]
        public ObjectId Id { get; set; }

    }
}
