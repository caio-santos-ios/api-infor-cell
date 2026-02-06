using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateServiceOrderItemDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Order = 1)]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Variação é obrigatória.")]
        [Display(Order = 2)]
        public string VariationId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
    }
}
