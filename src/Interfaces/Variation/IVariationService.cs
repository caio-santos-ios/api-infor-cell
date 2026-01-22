using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IVariationService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Variation?>> CreateAsync(CreateVariationDTO request);
        Task<ResponseApi<Variation?>> UpdateAsync(UpdateVariationDTO request);
        Task<ResponseApi<Variation>> DeleteAsync(string id);
    }
}