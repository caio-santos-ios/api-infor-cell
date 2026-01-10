using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateModelDTO : RequestDTO
    {
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A Marca é obrigatória.")]
        [Display(Order = 2)]
        public string BrandId { get; set; } = string.Empty;
    }
}