namespace api_infor_cell.src.Shared.DTOs
{
    public class CancelFiscalDocumentDTO : RequestDTO
    {
        public string FiscalDocumentId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}