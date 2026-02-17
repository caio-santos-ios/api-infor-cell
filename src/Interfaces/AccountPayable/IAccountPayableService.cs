using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IAccountPayableService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Models.AccountPayable?>> CreateAsync(CreateAccountPayableDTO request);
        Task<ResponseApi<Models.AccountPayable?>> UpdateAsync(UpdateAccountPayableDTO request);
        Task<ResponseApi<Models.AccountPayable?>> PayAsync(PayAccountPayableDTO request);
        Task<ResponseApi<Models.AccountPayable>> DeleteAsync(string id);
    }
}
