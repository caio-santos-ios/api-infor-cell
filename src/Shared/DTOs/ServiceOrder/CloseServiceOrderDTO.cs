namespace api_infor_cell.src.Shared.DTOs
{
    public class CloseServiceOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ClosedByUserId { get; set; } = string.Empty;
        public int WarrantyDays { get; set; } = 90;
        public DateTime? WarrantyUntil { get; set; }

        // Payment (only if not warranty)
        public string PaymentMethodId { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; } = string.Empty;
        public decimal NumberOfInstallments { get; set; } = 1;
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
    }
}
