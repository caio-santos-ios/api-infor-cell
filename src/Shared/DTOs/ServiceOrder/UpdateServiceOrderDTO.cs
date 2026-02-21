namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateServiceOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CancelReason { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;

        // Device
        public string DeviceType { get; set; } = string.Empty;
        public string BrandId { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string SerialImei { get; set; } = string.Empty;
        public string CustomerReportedIssue { get; set; } = string.Empty;
        public string UnlockPassword { get; set; } = string.Empty;
        public string Accessories { get; set; } = string.Empty;
        public string PhysicalCondition { get; set; } = string.Empty;

        // Laudo
        public string TechnicalReport { get; set; } = string.Empty;
        public string TestsPerformed { get; set; } = string.Empty;
        public string RepairStatus { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
    }
}
