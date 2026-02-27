using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Exchange : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        
        [BsonElement("salesOrderItemId")]
        public string SalesOrderItemId { get; set; } = string.Empty;
        
        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [BsonElement("forSale")]
        public string ForSale { get; set; } = string.Empty;
        
        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty;

        [BsonElement("originDescription")]
        public string OriginDescription { get; set; } = string.Empty;

        [BsonElement("releasedStock")]
        public bool ReleasedStock { get; set; } = false;

        [BsonElement("generateCashback")]
        public bool GenerateCashback { get; set; } = false;

        [BsonElement("balance")]
        public decimal Balance { get; set; } = 0;

        [BsonElement("typeBalance")]
        public string TypeBalance { get; set; } = string.Empty;
    }
}