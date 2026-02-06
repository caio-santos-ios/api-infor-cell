using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Box : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("openingValue")]
        public decimal OpeningValue {get; set;} = 0;

        [BsonElement("status")]
        public string Status {get; set;} = string.Empty;
    }
}