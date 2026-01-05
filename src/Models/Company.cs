using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Company : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("plan")]
        public string Plan { get; set; } = string.Empty; 

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty; 

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty; 

        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty; 

        [BsonElement("stateRegistration")]
        public string StateRegistration { get; set; } = string.Empty; 

        [BsonElement("municipalRegistration")]
        public string MunicipalRegistration { get; set; } = string.Empty; 

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("whatsapp")]
        public string Whatsapp { get; set; } = string.Empty;

        [BsonElement("photo")]
        public string Photo { get; set; } = string.Empty;

        [BsonElement("website")]
        public string Website { get; set; } = string.Empty;
    }
}