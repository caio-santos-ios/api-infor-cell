using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateServiceOrderDTO : RequestDTO
    {
        [Required(ErrorMessage = "O Cliente é obrigatório.")]
        [Display(Order = 1)]
        public string CustomerId { get; set; } = string.Empty;
        public string OpenedByUserId { get; set; } = string.Empty;

        // Device
        [Required(ErrorMessage = "O Tipo de Equipamento é obrigatório.")]
        [Display(Order = 2)]
        public string DeviceType { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Marca é obrigatório.")]
        [Display(Order = 3)]
        public string BrandId { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nº de Série/IMEI é obrigatório.")]
        [Display(Order = 4)]
        public string SerialImei { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A Prioridade é obrigatória.")]
        [Display(Order = 5)]
        public string Priority { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string CustomerReportedIssue { get; set; } = string.Empty;
        public string UnlockPassword { get; set; } = string.Empty;
        public string Accessories { get; set; } = string.Empty;
        public string PhysicalCondition { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
