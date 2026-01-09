using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateModuleEmployeeDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public List<Module> Modules {get;set;} = [];
    }
}
