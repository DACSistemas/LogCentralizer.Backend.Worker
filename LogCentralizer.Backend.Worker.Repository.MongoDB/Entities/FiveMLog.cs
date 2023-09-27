using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    [BsonCollection("Logs")]
    public class FiveMLog : Document
    {
        [BsonElement("payload")]
        public string Payload { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("room")]
        public string Room { get; set; }

        [BsonElement("user_id")]
        public int? UserId { get; set; }

        [BsonElement("target_id")]
        public int? TargetId { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("event_time")]
        public DateTime EventTime { get; set; }

    }
}
