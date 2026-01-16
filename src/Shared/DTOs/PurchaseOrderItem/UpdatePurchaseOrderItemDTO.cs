using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdatePurchaseOrderItemDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Order = 1)]
        public string ProductId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Fornecedor é obrigatório.")]
        [Display(Order = 2)]
        public string SupplierId { get; set; } = string.Empty;

        public string PurchaseOrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Custo é obrigatório.")]
        [Display(Order = 3)]
        public decimal Cost { get; set; }
        public decimal CostDiscount { get; set; }
        
        [Required(ErrorMessage = "A Quantidade é obrigatória.")]
        [Display(Order = 4)]
        public decimal Quantity { get; set; }
        
        [Required(ErrorMessage = "O Preço é obrigatório.")]
        [Display(Order = 5)]
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public string MoveStock {get;set;} = string.Empty;
        public List<Variation> Variations {get;set;} = [];
    }
}