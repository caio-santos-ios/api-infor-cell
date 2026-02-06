using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Variation : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("items")]
        public List<VariationItem> Items { get; set; } = [];
    }

    public class VariationItem
    {
        [BsonElement("code")]
        public string Code {get;set;} = string.Empty;

        [BsonElement("key")]
        public string Key {get;set;} = string.Empty;
        
        [BsonElement("value")]
        public string Value {get;set;} = string.Empty;

        [BsonElement("active")]
        public bool Active {get;set;} = true;

        [BsonElement("deleted")]
        public bool Deleted {get;set;} = false;

        [BsonElement("serial")]
        public List<VariationItemSerial> Serial {get;set;} = [];
    }
}