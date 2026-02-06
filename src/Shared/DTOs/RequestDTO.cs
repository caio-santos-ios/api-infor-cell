namespace api_infor_cell.src.Shared.DTOs
{
    public class RequestDTO
    {
        public string CreatedBy {get;set;} = string.Empty;
        public string UpdatedBy {get;set;} = string.Empty;
        public string DeletedBy {get;set;} = string.Empty;
        public string Company {get;set;} = string.Empty;
        public string Store {get;set;} = string.Empty;
        public string Plan {get;set;} = string.Empty;
    }
}