using System.ComponentModel.DataAnnotations;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateExchangeDTO : ModelMasterBase
    {
        public string Id { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string OriginDescription { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public List<VariationProduct> Variations {get;set;} = [];
        public List<string> VariationsCode { get; set; } = [];

        // TROCA
        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Order = 1)]
        public string ProductId { get; set; } = string.Empty;
        public string SalesOrderItemId { get; set; } = string.Empty;
        public string ForSale { get; set; } = string.Empty;
    }
}
