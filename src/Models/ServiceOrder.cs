using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class ServiceOrder : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("openedByUserId")]
        public string OpenedByUserId { get; set; } = string.Empty;

        [BsonElement("openedAt")]
        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("closedAt")]
        public DateTime? ClosedAt { get; set; }

        [BsonElement("closedByUserId")]
        public string ClosedByUserId { get; set; } = string.Empty;

        // open | analysis | waiting_approval | waiting_part | in_repair | ready | closed | cancelled
        [BsonElement("status")]
        public string Status { get; set; } = "open";

        [BsonElement("isWarrantyInternal")]
        public bool IsWarrantyInternal { get; set; } = false;

        [BsonElement("warrantyUntil")]
        public DateTime? WarrantyUntil { get; set; }

        [BsonElement("warrantyDays")]
        public int WarrantyDays { get; set; } = 90;

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; } = 0;

        [BsonElement("discountValue")]
        public decimal DiscountValue { get; set; } = 0;

        [BsonElement("discountType")]
        public string DiscountType { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("cancelReason")]
        public string CancelReason { get; set; } = string.Empty;

        [BsonElement("warrantyOverrideReason")]
        public string WarrantyOverrideReason { get; set; } = string.Empty;

        [BsonElement("device")]
        public ServiceOrderDevice Device { get; set; } = new();

        [BsonElement("payment")]
        public ServiceOrderPayment Payment { get; set; } = new();

        [BsonElement("laudo")]
        public ServiceOrderLaudo Laudo { get; set; } = new();
    }

    public class ServiceOrderDevice
    {
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("brandId")]
        public string BrandId { get; set; } = string.Empty;

        // [BsonElement("brandName")]
        // public string BrandName { get; set; } = string.Empty;

        [BsonElement("modelId")]
        // public string ModelId { get; set; } = string.Empty;

        // [BsonElement("modelName")]
        public string ModelName { get; set; } = string.Empty;

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("serialImei")]
        public string SerialImei { get; set; } = string.Empty;

        [BsonElement("customerReportedIssue")]
        public string CustomerReportedIssue { get; set; } = string.Empty;

        [BsonElement("unlockPassword")]
        public string UnlockPassword { get; set; } = string.Empty;

        [BsonElement("accessories")]
        public string Accessories { get; set; } = string.Empty;

        [BsonElement("physicalCondition")]
        public string PhysicalCondition { get; set; } = string.Empty;
    }

    public class ServiceOrderPayment
    {
        [BsonElement("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        // [BsonElement("paymentMethodName")]
        // public string PaymentMethodName { get; set; } = string.Empty;

        [BsonElement("numberOfInstallments")]
        public decimal NumberOfInstallments { get; set; } = 1;

        [BsonElement("paidAt")]
        public DateTime? PaidAt { get; set; }

        [BsonElement("paidByUserId")]
        public string PaidByUserId { get; set; } = string.Empty;
    }

    public class ServiceOrderLaudo
    {
        [BsonElement("technicalReport")]
        public string TechnicalReport { get; set; } = string.Empty;

        [BsonElement("testsPerformed")]
        public string TestsPerformed { get; set; } = string.Empty;

        [BsonElement("repairStatus")]
        public string RepairStatus { get; set; } = string.Empty;
    }
}