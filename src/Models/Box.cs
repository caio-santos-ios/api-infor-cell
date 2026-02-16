using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Box : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("openingValue")]
        public decimal OpeningValue {get; set;} = 0;
        
        [BsonElement("closingValue")]
        public decimal ClosingValue {get; set;} = 0;

        [BsonElement("value")]
        public decimal Value {get; set;} = 0;

        [BsonElement("status")]
        public string Status {get; set;} = string.Empty;

        [BsonElement("twoSteps")]
        public string TwoSteps {get; set;} = string.Empty;
        
        [BsonElement("opendBy")]
        public string OpendBy {get; set;} = string.Empty;
        
        [BsonElement("closedBy")]
        public string ClosedBy {get; set;} = string.Empty;

        [BsonElement("sangria")]
        public List<decimal> Sangria {get; set;} = [];
        
        [BsonElement("reinforce")]
        public List<decimal> Reinforce {get; set;} = [];
    }
}