using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IFiscalDocumentRepository
    {
        Task<ResponseApi<FiscalDocument?>> GetByOriginAsync(string originId, string originType);
        Task<ResponseApi<FiscalDocument?>> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<FiscalDocument> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<FiscalDocument> pagination);
        Task<ResponseApi<FiscalDocument?>> CreateAsync(FiscalDocument doc);
        Task<ResponseApi<FiscalDocument?>> UpdateAsync(FiscalDocument doc);
        Task<ResponseApi<FiscalEvent?>> CreateEventAsync(FiscalEvent evt);
        Task<ResponseApi<FiscalConfig?>> GetConfigByStoreAsync(string storeId);
        Task<ResponseApi<FiscalConfig?>> SaveConfigAsync(FiscalConfig config);
        Task<long> GetNextNumberAsync(string storeId, int model);
    }
}