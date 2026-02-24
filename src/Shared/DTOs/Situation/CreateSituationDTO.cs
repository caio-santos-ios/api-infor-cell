using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateSituationDTO : RequestDTO
    {
        public string Name {get; set;} = string.Empty;
        public Style Style {get; set;} = new();
        public bool Start {get; set;}
        public bool End {get; set;}
        public bool Quite {get; set;}
        public bool GenerateFinancial {get; set;}
        public bool AppearsOnPanel {get; set;}
    }
}