using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateSalesOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Vendedor é obrigatório.")]
        [Display(Order = 1)]
        public string SellerId { get; set; } = string.Empty;

        public string VariationId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public bool CreateItem {get; set;} = false;
        public string Barcode { get; set; } = string.Empty;
    }
}
