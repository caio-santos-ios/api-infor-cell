using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class LogApi : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        [BsonElement("table")]
        public string Table { get; set; } = string.Empty;
        
        [BsonElement("originId")]
        public string OriginId { get; set; } = string.Empty;
        
        [BsonElement("originSecondaryId")]
        public string OriginSecondaryId { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
        
        [BsonElement("response")]
        public string Response { get; set; } = string.Empty;
        
        [BsonElement("responseMessage")]
        public string ResponseMessage { get; set; } = string.Empty;
        
    }
}