using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdatePurchaseOrderDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "A Data é obrigatória.")]
        [Display(Order = 1)]
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}