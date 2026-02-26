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
        
        [BsonElement("barcode")]
        public string Barcode { get; set; } = string.Empty;
        
        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("supplierId")]
        public string SupplierId { get; set; } = string.Empty;
        
        [BsonElement("purchaseOrderItemId")]
        public string PurchaseOrderItemId { get; set; } = string.Empty;

        [BsonElement("origin")]
        public string Origin { get; set; } = string.Empty;
        
        [BsonElement("originDescription")]
        public string OriginDescription { get; set; } = string.Empty;

        [BsonElement("cost")]
        public decimal Cost { get; set; }

        [BsonElement("costDiscount")]
        public decimal CostDiscount { get; set; }

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("quantityAvailable")]
        public decimal QuantityAvailable { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("priceDiscount")]
        public decimal PriceDiscount { get; set; }

        [BsonElement("hasProductVariations")]
        public string HasProductVariations { get; set; } = string.Empty;
        
        [BsonElement("hasProductSerial")]
        public string HasProductSerial { get; set; } = string.Empty;
        
        [BsonElement("variations")]
        public List<VariationProduct> Variations {get;set;} = [];
        
        [BsonElement("variationsCode")]
        public List<string> VariationsCode { get; set; } = [];
        
        [BsonElement("forSale")]
        public string ForSale { get; set; } = string.Empty;

        [BsonElement("originId")]
        public string OriginId { get; set; } = string.Empty;
        
        [BsonElement("isReserved")]
        public bool IsReserved { get; set; } = false;
        
        [BsonElement("customerIdReserved")]
        public List<string> CustomerIdReserved { get; set; } = [];
    }
}