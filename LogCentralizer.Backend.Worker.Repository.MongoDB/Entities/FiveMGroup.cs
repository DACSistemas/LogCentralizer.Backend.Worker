using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    [BsonCollection("Groups")]
    public class FiveMGroup : Document
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;

        [BsonElement("cities")]
        public List<FiveMCity> Cities { get; set; } = new();
    }
}
