using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models.Base
{
    public class ModelMasterBase : ModelBase
    {        
        [BsonElement("company")]
        public string Company {get;set;} = string.Empty;
        
        [BsonElement("store")]
        public string Store {get;set;} = string.Empty;
       
        [BsonElement("plan")]
        public string Plan {get;set;} = string.Empty;
    }
}