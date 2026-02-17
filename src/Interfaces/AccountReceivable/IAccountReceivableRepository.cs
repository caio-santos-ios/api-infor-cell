using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IAccountReceivableRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Models.AccountReceivable> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Models.AccountReceivable?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Models.AccountReceivable> pagination);
        Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
        Task<ResponseApi<Models.AccountReceivable?>> CreateAsync(Models.AccountReceivable accountReceivable);
        Task<ResponseApi<Models.AccountReceivable?>> UpdateAsync(Models.AccountReceivable accountReceivable);
        Task<ResponseApi<Models.AccountReceivable?>> PayAsync(Models.AccountReceivable accountReceivable);
        Task<ResponseApi<Models.AccountReceivable>> DeleteAsync(string id);
    }
}
