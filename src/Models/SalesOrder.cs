using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class SalesOrder : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;
        
        [BsonElement("customerId")]
        public string CustomerId { get; set; } = string.Empty;
        
        [BsonElement("sellerId")]
        public string SellerId { get; set; } = string.Empty;

        [BsonElement("total")]
        public decimal Total { get; set; }
        
        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("discountValue")]
        public decimal DiscountValue { get; set; }

        [BsonElement("discountType")]
        public string DiscountType { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = string.Empty;
        
        [BsonElement("payment")]
        public Payment Payment {get; set;} = new();
    }

    public class Payment 
    {
        public string PaymentMethodId { get; set; } = string.Empty;
        public decimal NumberOfInstallments { get; set; }
        public decimal Freight { get; set; }
        public string Currier { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
    }
}