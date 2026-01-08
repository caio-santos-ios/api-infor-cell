namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateEmployeeDTO
    {
        public string Name {get;set;} = string.Empty;
        public string UserName {get;set;} = string.Empty;
        public string Email {get;set;} = string.Empty;
        public string Password {get;set;} = string.Empty;
        public string Phone {get;set;} = string.Empty;
        public string Whatsapp {get;set;} = string.Empty;
        public bool Blocked {get;set;} = false;
        public bool Admin {get;set;} = false;
        public string CodeAccess {get;set;} = string.Empty;
        public bool ValidatedAccess {get;set;} = false;
        public DateTime? CodeAccessExpiration { get; set; }
        public string Photo {get;set;} = string.Empty;
        public string Company {get;set;} = string.Empty;
        public string Store {get;set;} = string.Empty;
        public List<string> Companies {get;set;} = [];
        public List<string> Stores {get;set;} = [];
        public List<Module> Modules {get;set;} = [];
        public string Plan { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty; 
        public int? Age { get; set; } 
    }
}