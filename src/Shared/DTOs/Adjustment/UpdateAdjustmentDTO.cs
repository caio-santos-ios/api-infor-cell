using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateAdjustmentDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string HasSerial { get; set; } = string.Empty;
        public string HasVariations { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal CostDiscount { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceDiscount { get; set; }
        public List<VariationProduct> Variations {get;set;} = [];
        public List<string> VariationsCode { get; set; } = [];
    }
}
