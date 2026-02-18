using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace api_infor_cell.src.Models
{
    public class ChartOfAccounts : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("code")]
        [Required(ErrorMessage = "Código é obrigatório")]
        [Display(Order = 1)]
        public string Code { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [Display(Order = 2)]
        public string Name { get; set; } = string.Empty;

        [BsonElement("parentId")]
        [Display(Order = 3)]
        public string? ParentId { get; set; }

        [BsonElement("type")]
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [Display(Order = 4)]
        public string Type { get; set; } = string.Empty;

        [BsonElement("dreCategory")]
        [Display(Order = 5)]
        public string? DreCategory { get; set; } = string.Empty;

        [BsonElement("showInDre")]
        [Display(Order = 6)]
        public bool ShowInDre { get; set; } = true;

        [BsonElement("description")]
        [Display(Order = 7)]
        public string Description { get; set; } = string.Empty;

        [BsonElement("level")]
        [Display(Order = 8)]
        public int Level { get; set; }

        [BsonElement("isAnalytical")]
        [Display(Order = 9)]
        public bool IsAnalytical { get; set; } = false;
    }
}