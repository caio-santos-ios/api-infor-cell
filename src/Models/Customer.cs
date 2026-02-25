using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Customer : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;
        
        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("document")]
        public string Document { get; set; } = string.Empty; 

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("cashbacks")]
        public List<CashbackCustomer> Cashbacks {get;set;} = [];

        [BsonElement("totalCashback")]
        public decimal TotalCashback { get; set; }
        
        [BsonElement("totalCurrentCashback")]
        public decimal TotalCurrentCashback { get; set; }
    }

    public class CashbackCustomer
    {
        [BsonElement("responsible")]
        public string Responsible { get; set; } = string.Empty;
        
        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        
        [BsonElement("originDescription")]
        public string OriginDescription { get; set; } = string.Empty;
        
        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty;
        
        [BsonElement("originId")]
        public string OriginId { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; } = new();

        [BsonElement("value")]
        public decimal Value { get; set; }

        [BsonElement("currentValue")]
        public decimal CurrentValue { get; set; }

        [BsonElement("available")]
        public bool Available { get; set; } = true;
    }
}