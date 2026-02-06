using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IStockService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Stock?>> CreateAsync(CreateStockDTO request);
        Task<ResponseApi<Stock?>> UpdateAsync(UpdateStockDTO request);
        Task<ResponseApi<Stock>> DeleteAllByProductAsync(DeleteDTO request);
        Task<ResponseApi<Stock>> DeleteAsync(string id);
    }
}