using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Collections;
using MongoDB.Bson.Serialization.Attributes;

namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    [BsonCollection("Categories_Roles")]
    public class CategoryRole : Document
    {
        [BsonElement("category")]
        public string Category { get; set; }
        
        [BsonElement("category_name")]
        public string CategoryName { get; set; }

        [BsonElement("min_role")]
        public string MinimalRole { get; set; } = null;

        [BsonElement("icon")]
        public string Icon { get; set; } = null;


    }
}
