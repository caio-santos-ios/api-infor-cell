using api_infor_cell.src.Models;

namespace api_infor_cell.src.Interfaces
{
    /// <summary>
    /// Interface plugável para o provedor SEFAZ.
    /// Permite trocar de biblioteca (Unimake.DFe, DFe.NET, SOAP direto) sem mexer no Service.
    /// </summary>
    public interface ISefazProvider
    {
        /// <summary>Gera, assina e valida o XML da NF-e/NFC-e</summary>
        Task<SefazBuildResult> BuildAndSignAsync(FiscalDocument doc, FiscalConfig config);

        /// <summary>Transmite o lote para a SEFAZ e retorna o resultado</summary>
        Task<SefazTransmitResult> TransmitAsync(string signedXml, FiscalDocument doc, FiscalConfig config);

        /// <summary>Consulta o status de um documento já transmitido pelo recibo ou chave de acesso</summary>
        Task<SefazTransmitResult> QueryStatusAsync(string accessKeyOrReceipt, FiscalDocument doc, FiscalConfig config);

        /// <summary>Gera e transmite evento de cancelamento</summary>
        Task<SefazEventResult> CancelAsync(FiscalDocument doc, string reason, FiscalConfig config);

        /// <summary>Gera e transmite Carta de Correção Eletrônica</summary>
        Task<SefazEventResult> SendCorrectionLetterAsync(FiscalDocument doc, string correctionText, int sequenceNumber, FiscalConfig config);
    }

    public class SefazBuildResult
    {
        public bool Success { get; set; }
        public string SignedXml { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class SefazTransmitResult
    {
        public bool Authorized { get; set; }
        public bool Rejected { get; set; }
        public bool Denied { get; set; }
        public bool CommunicationError { get; set; }
        public string ReturnCode { get; set; } = string.Empty;
        public string ReturnMessage { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public string AuthorizedXml { get; set; } = string.Empty;
    }

    public class SefazEventResult
    {
        public bool Success { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public string EventXml { get; set; } = string.Empty;
        public string ReturnCode { get; set; } = string.Empty;
        public string ReturnMessage { get; set; } = string.Empty;
    }
}