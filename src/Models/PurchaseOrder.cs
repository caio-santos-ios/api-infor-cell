using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class PurchaseOrder : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;
        
        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
        
        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }
        
        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }
    }
}