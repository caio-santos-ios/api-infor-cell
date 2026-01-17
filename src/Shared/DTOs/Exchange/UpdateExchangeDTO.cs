using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateExchangeDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Loja de Origem é obrigatório.")]
        [Display(Order = 1)]
        public string StoreOriginId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Loja de Destino é obrigatório.")]
        [Display(Order = 2)]
        public string StoreDestinationId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public string PurchaseOrderItemId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
