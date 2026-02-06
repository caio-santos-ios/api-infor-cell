using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateSalesOrderItemDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Order = 1)]
        public string ProductId { get; set; } = string.Empty;
        public string SalesOrderId { get; set; } = string.Empty;
        public string VariationId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public bool CreateItem {get; set;} = false;
        public string Barcode { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
    }
}