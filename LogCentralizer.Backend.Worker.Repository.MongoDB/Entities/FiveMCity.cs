using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    public class FiveMCity : Document
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("active")]
        public bool Active { get; set; } = false;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("queue_name")]
        public string QueueName { get; set; }

        [BsonElement("exchange_name")]
        public string ExchangeName { get; set; }
    }
}
