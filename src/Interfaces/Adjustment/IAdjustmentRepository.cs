using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IAdjustmentRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Adjustment> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Adjustment?>> GetByIdAsync(string id);
        Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Adjustment> pagination);
        Task<ResponseApi<Adjustment?>> CreateAsync(Adjustment address);
        Task<ResponseApi<Adjustment?>> UpdateAsync(Adjustment address);
        Task<ResponseApi<Adjustment>> DeleteAsync(string id);
    }
}
