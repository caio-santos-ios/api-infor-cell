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
        
        [BsonElement("userId")]
        public string UserId {get;set;} = string.Empty;

        [BsonElement("phone")]
        public string Phone {get;set;} = string.Empty;
        
        [BsonElement("whatsapp")]
        public string Whatsapp {get;set;} = string.Empty;
        
        [BsonElement("type")]
        public string Type { get; set; } = string.Empty; 
        
        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty; 
        
        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty; 
        
        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [BsonElement("calendar")]
        public Calendar Calendar { get; set; } = new(); 
    }

    public class Calendar
    {
        [BsonElement("monday")]
        public List<string> Monday {get;set;} = [];

        [BsonElement("tuesday")]
        public List<string> Tuesday {get;set;} = [];

        [BsonElement("wednesday")]
        public List<string> Wednesday {get;set;} = [];
        
        [BsonElement("thursday")]
        public List<string> Thursday {get;set;} = [];
        
        [BsonElement("friday")]
        public List<string> Friday {get;set;} = [];

        [BsonElement("saturday")]
        public List<string> Saturday {get;set;} = [];

        [BsonElement("sunday")]
        public List<string> Sunday {get;set;} = [];
    } 
}