using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateStockDTO : RequestDTO
    {
        public string Code { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string SupplierId { get; set; } = string.Empty;
        public string PurchaseOrderItemId { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal CostDiscount { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public List<Variation> Variations {get;set;} = [];
    }
}