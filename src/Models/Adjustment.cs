using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Adjustment : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        
        [BsonElement("barcode")]
        public string Barcode { get; set; } = string.Empty;
        
        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

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
        public List<VariationProduct> Variations {get;set;} = [];
        
        [BsonElement("variationsCode")]
        public List<string> VariationsCode { get; set; } = [];
    }
}