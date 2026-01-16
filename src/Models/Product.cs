using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Product : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("categoryId")]
        public string CategoryId { get; set; } = string.Empty;

        [BsonElement("imei")]
        public string Imei { get; set; } = string.Empty;

        [BsonElement("modelId")]
        public string ModelId { get; set; } = string.Empty;

        [BsonElement("moveStock")]
        public bool MoveStock { get; set; }

        [BsonElement("quantityStock")]
        public int QuantityStock { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("priceDiscount")]
        public decimal PriceDiscount { get; set; }

        [BsonElement("priceTotal")]
        public decimal PriceTotal { get; set; }

        [BsonElement("costPrice")]
        public decimal CostPrice { get; set; }

        [BsonElement("expenseCostPrice")]
        public decimal ExpenseCostPrice { get; set; }
        
        [BsonElement("variations")]
        public List<Variation> Variations {get;set;} = [];
    }

    public class Variation
    {
        [BsonElement("sequence")]
        public int Sequence {get;set;}
        
        [BsonElement("key")]
        public string Key {get;set;} = string.Empty;
        
        [BsonElement("value")]
        public string Value {get;set;} = string.Empty;
    }
}