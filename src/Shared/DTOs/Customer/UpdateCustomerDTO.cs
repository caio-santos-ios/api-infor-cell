using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateCustomerDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Order = 1)]
        public string CorporateName { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Documento é obrigatório.")]
        [Display(Order = 2)]
        public string Document { get; set; } = string.Empty; 

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Order = 3)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        [Display(Order = 4)]
        public string Phone { get; set; } = string.Empty;
    }
}
