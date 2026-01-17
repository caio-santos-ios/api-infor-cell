using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Transfer : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("storeOriginId")]
        public string StoreOriginId { get; set; } = string.Empty;

        [BsonElement("storeDestinationId")]
        public string StoreDestinationId { get; set; } = string.Empty;
        
        [BsonElement("stockId")]
        public string StockId { get; set; } = string.Empty;

        [BsonElement("purchaseOrderItemId")]
        public string PurchaseOrderItemId { get; set; } = string.Empty;
    
        [BsonElement("quantity")]
        public decimal Quantity { get; set; }
    }
}