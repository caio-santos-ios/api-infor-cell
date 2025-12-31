using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Contact : ModelBase 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;
        
        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;
        
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("department")]
        public string Department { get; set; } = string.Empty;

        [BsonElement("position")]
        public string Position { get; set; } = string.Empty;
        
        [BsonElement("parentId")]
        public string ParentId { get; set; } = string.Empty;
        
        [BsonElement("parent")]
        public string Parent { get; set; } = string.Empty;
    }
}