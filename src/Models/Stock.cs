using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Stock : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;
        
        [BsonElement("serialNumber")]
        public string SerialNumber { get; set; } = string.Empty;
        
        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("supplierId")]
        public string SupplierId { get; set; } = string.Empty;
        
        [BsonElement("purchaseOrderItemId")]
        public string PurchaseOrderItemId { get; set; } = string.Empty;

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
        
        [BsonElement("variations")]
        public List<Variation> Variations {get;set;} = [];
    }
}