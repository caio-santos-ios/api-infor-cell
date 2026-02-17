using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IAccountReceivableService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<AccountReceivable?>> CreateAsync(CreateAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable?>> UpdateAsync(UpdateAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable?>> PayAsync(PayAccountReceivableDTO request);
        Task<ResponseApi<AccountReceivable>> DeleteAsync(string id);
    }
}
