using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdatePaymentMethodDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        [Required(ErrorMessage = "O Nº máximo de parcelas é obrigatório.")]
        [Display(Order = 2)]
        public int NumberOfInstallments { get; set; }
    }
}
