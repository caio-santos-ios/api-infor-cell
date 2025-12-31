using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Supplier : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = "F";

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty;
        
        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;

        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("stateRegistration")]
        public string StateRegistration { get; set; } = string.Empty;

        [BsonElement("municipalRegistration")]
        public string MunicipalRegistration { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
        
        [BsonElement("effectiveDate")]
        public DateTime? EffectiveDate { get; set; }
    }
}