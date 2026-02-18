using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class GenerateDreDTO
    {
        [Required(ErrorMessage = "Data inicial é obrigatória")]
        [Display(Order = 1)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Data final é obrigatória")]
        [Display(Order = 2)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Regime é obrigatório")]
        [Display(Order = 3)]
        public string Regime { get; set; } = "competencia";
    }
}