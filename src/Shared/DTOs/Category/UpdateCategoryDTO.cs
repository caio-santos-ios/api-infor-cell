using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateCategoryDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Subcategory> SubCategories { get; set; } = [];
    }
}
