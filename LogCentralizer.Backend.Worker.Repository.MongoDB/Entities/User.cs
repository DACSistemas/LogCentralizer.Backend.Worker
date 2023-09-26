using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    [BsonCollection("Users")]
    public class User : Document
    {

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password_hash")]
        public string Password { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
