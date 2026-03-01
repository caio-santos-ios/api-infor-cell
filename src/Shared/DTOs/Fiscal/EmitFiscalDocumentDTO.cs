namespace api_infor_cell.src.Shared.DTOs
{
    public class EmitFiscalDocumentDTO : RequestDTO
    {
        /// <summary>ID da venda ou OS</summary>
        public string OriginId { get; set; } = string.Empty;

        /// <summary>sales_order | service_order</summary>
        public string OriginType { get; set; } = string.Empty;

        /// <summary>55 = NF-e | 65 = NFC-e</summary>
        public int Model { get; set; } = 65;
    }
}