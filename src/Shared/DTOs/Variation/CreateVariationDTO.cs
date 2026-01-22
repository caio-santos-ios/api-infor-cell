using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateVariationDTO : RequestDTO
    {        
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;

        public List<VariationItem> Items { get; set; } = [];
    }
}
