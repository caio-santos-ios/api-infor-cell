using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateGenericTableDTO : RequestDTO
    {
        public string Table {get;set;} = string.Empty;
        public string Code {get;set;} = string.Empty;
        
        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        [Display(Order = 1)]
        public string Description {get;set;} = string.Empty;
        public bool CodeAutomatic {get;set;} = false;
    }
}