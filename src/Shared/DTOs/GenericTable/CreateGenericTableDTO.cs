namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateGenericTableDTO : RequestDTO
    {

        public string Table {get;set;} = string.Empty;

        public string Code {get;set;} = string.Empty;

        public string Description {get;set;} = string.Empty;
    }
}