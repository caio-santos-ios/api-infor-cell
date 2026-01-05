using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IPlanRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Plan?>> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<Plan?>> CreateAsync(Plan address);
        Task<ResponseApi<Plan?>> UpdateAsync(Plan address);
        Task<ResponseApi<Plan>> DeleteAsync(string id);
    }
}