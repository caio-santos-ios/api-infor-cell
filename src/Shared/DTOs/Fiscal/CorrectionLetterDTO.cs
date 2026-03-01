namespace api_infor_cell.src.Shared.DTOs
{
    public class CorrectionLetterDTO : RequestDTO
    {
        public string FiscalDocumentId { get; set; } = string.Empty;
        public string CorrectionText { get; set; } = string.Empty;
    }
}
