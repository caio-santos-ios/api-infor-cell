using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateProductDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Imei { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A Categoria é obrigatória.")]
        [Display(Order = 2)]
        public string CategoryId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Modelo é obrigatório.")]
        [Display(Order = 3)]
        public string ModelId { get; set; } = string.Empty;
        public bool MoveStock { get; set; }
        public int QuantityStock { get; set; }
        
        [Required(ErrorMessage = "O Preço é obrigatório.")]
        [Display(Order = 5)]
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public decimal PriceTotal { get; set; }
        
        [Required(ErrorMessage = "O Custo é obrigatório.")]
        [Display(Order = 4)]
        public decimal CostPrice { get; set; }
        public decimal ExpenseCostPrice { get; set; }
        public List<Variation> Variations {get;set;} = [];
    }
}