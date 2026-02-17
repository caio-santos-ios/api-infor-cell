using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IAccountPayableRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Models.AccountPayable> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Models.AccountPayable?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Models.AccountPayable> pagination);
        Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
        Task<ResponseApi<Models.AccountPayable?>> CreateAsync(Models.AccountPayable accountPayable);
        Task<ResponseApi<Models.AccountPayable?>> UpdateAsync(Models.AccountPayable accountPayable);
        Task<ResponseApi<Models.AccountPayable?>> PayAsync(Models.AccountPayable accountPayable);
        Task<ResponseApi<Models.AccountPayable>> DeleteAsync(string id);
    }
}
