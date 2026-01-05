using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IPlanService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Plan?>> CreateAsync(CreatePlanDTO request);
    Task<ResponseApi<Plan?>> UpdateAsync(UpdatePlanDTO request);
    Task<ResponseApi<Plan>> DeleteAsync(string id);
}
}