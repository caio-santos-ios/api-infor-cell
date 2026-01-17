using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class PurchaseOrderItem : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [BsonElement("supplierId")]
        public string SupplierId { get; set; } = string.Empty;
        
        [BsonElement("purchaseOrderId")]
        public string PurchaseOrderId { get; set; } = string.Empty;
        
        [BsonElement("cost")]
        public decimal Cost { get; set; }

        [BsonElement("costDiscount")]
        public decimal CostDiscount { get; set; }

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("priceDiscount")]
        public decimal PriceDiscount { get; set; }
        
        [BsonElement("margin")]
        public decimal Margin { get; set; }
        
        [BsonElement("netProfit")]
        public decimal NetProfit { get; set; }

        [BsonElement("moveStock")]
        public string MoveStock {get;set;} = string.Empty;

        [BsonElement("variations")]
        public List<Variation> Variations {get;set;} = [];
    }
}