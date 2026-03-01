using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IFiscalDocumentService
    {
        Task<ResponseApi<FiscalDocument?>> EmitAsync(EmitFiscalDocumentDTO request, string userId);
        Task<ResponseApi<FiscalDocument?>> CancelAsync(CancelFiscalDocumentDTO request, string userId);
        Task<ResponseApi<FiscalEvent?>> SendCorrectionLetterAsync(CorrectionLetterDTO request, string userId);
        Task<ResponseApi<FiscalDocument?>> RetryAsync(string fiscalDocumentId, string userId);
        Task<ResponseApi<dynamic?>> GetByOriginAsync(string originId, string originType);
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<FiscalConfig?>> SaveConfigAsync(SaveFiscalConfigDTO request, string userId);
        Task<ResponseApi<FiscalConfig?>> GetConfigAsync(string store);
    }
}