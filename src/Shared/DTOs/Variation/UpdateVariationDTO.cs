using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateVariationDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        [Display(Order = 6)]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string SerialAction { get; set; } = string.Empty;
        public List<VariationItem> Items { get; set; } = [];
    }
}
