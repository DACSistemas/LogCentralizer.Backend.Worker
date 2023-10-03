using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    [BsonCollection("Rooms_Roles")]
    public class RoomRole : Document
    {
        [BsonElement("room")]
        public string Room { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = null;

        [BsonElement("min_role")]
        public string MinimalRole { get; set; } = null;

    }
}
