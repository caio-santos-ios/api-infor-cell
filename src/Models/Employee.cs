using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Employee : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("name")]
        public string Name {get;set;} = string.Empty;

        [BsonElement("userName")]
        public string UserName {get;set;} = string.Empty;

        [BsonElement("email")]
        public string Email {get;set;} = string.Empty;

        [BsonElement("password")]
        public string Password {get;set;} = string.Empty;

        [BsonElement("phone")]
        public string Phone {get;set;} = string.Empty;
        
        [BsonElement("whatsapp")]
        public string Whatsapp {get;set;} = string.Empty;

        [BsonElement("blocked")]
        public bool Blocked {get;set;} = false;
        
        [BsonElement("admin")]
        public bool Admin {get;set;} = false;
        
        [BsonElement("codeAccess")]
        public string CodeAccess {get;set;} = string.Empty;

        [BsonElement("validatedAccess")]
        public bool ValidatedAccess {get;set;} = false;

        [BsonElement("codeAccessExpiration")]
        public DateTime? CodeAccessExpiration { get; set; }

        [BsonElement("photo")]
        public string Photo {get;set;} = string.Empty;
        
        [BsonElement("company")]
        public string Company {get;set;} = string.Empty;
        
        [BsonElement("store")]
        public string Store {get;set;} = string.Empty;

        [BsonElement("companies")]
        public List<string> Companies {get;set;} = [];
        
        [BsonElement("stores")]
        public List<string> Stores {get;set;} = [];
        
        [BsonElement("modules")]
        public List<Module> Modules {get;set;} = [];

        [BsonElement("plan")]
        public string Plan { get; set; } = string.Empty; 
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty; 
        
        [BsonElement("age")]
        public int? Age { get; set; } 
    }
}