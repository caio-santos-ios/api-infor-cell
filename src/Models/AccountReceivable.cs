using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class AccountReceivable : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("originId")]
        public string OriginId { get; set; } = string.Empty;

        [BsonElement("originType")]
        public string OriginType { get; set; } = "manual";

        [BsonElement("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [BsonElement("paymentMethodName")]
        public string PaymentMethodName { get; set; } = string.Empty;

        [BsonElement("chartOfAccountsId")]
        public string ChartOfAccountsId { get; set; } = string.Empty;

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("amountPaid")]
        public decimal AmountPaid { get; set; }

        [BsonElement("installmentNumber")]
        public int InstallmentNumber { get; set; } = 1;

        [BsonElement("totalInstallments")]
        public int TotalInstallments { get; set; } = 1;

        [BsonElement("dueDate")]
        public DateTime DueDate { get; set; }

        [BsonElement("paidAt")]
        public DateTime? PaidAt { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "open";

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }
}