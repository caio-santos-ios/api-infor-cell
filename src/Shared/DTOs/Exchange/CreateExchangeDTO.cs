using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateExchangeDTO : RequestDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string OriginDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Order = 1)]
        public string ProductId { get; set; } = string.Empty;
        public string SalesOrderItemId { get; set; } = string.Empty;
        public string ForSale { get; set; } = string.Empty;
        public bool GenerateCashback { get; set; } = false;
        public string HasVariations { get; set; } = string.Empty;
        public string HasSerial { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public string TypeBalance { get; set; } = string.Empty;
    }
}