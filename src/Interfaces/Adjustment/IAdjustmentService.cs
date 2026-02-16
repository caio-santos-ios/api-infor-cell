using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IAdjustmentService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Adjustment?>> CreateAsync(CreateAdjustmentDTO request);
        Task<ResponseApi<Adjustment?>> UpdateAsync(UpdateAdjustmentDTO request);
        Task<ResponseApi<Adjustment>> DeleteAsync(string id);
    }
}