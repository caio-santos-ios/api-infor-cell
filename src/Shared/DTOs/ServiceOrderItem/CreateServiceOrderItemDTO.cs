
namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateServiceOrderItemDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string VariationId { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public bool CreateItem {get; set;} = false;
        public string Barcode { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
    }
}