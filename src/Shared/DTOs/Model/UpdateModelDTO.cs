using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateModelDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BrandId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string GroupFather { get; set; } = string.Empty;
    }
}
