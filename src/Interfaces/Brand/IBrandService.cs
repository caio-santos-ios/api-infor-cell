using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IBrandService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Brand?>> CreateAsync(CreateBrandDTO request);
    Task<ResponseApi<Brand?>> UpdateAsync(UpdateBrandDTO request);
    Task<ResponseApi<Brand>> DeleteAsync(string id);
}
}