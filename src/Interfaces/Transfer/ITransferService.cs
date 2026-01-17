using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ITransferService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Transfer?>> CreateAsync(CreateTransferDTO request);
    Task<ResponseApi<Transfer?>> UpdateAsync(UpdateTransferDTO request);
    Task<ResponseApi<Transfer>> DeleteAsync(string id);
}
}