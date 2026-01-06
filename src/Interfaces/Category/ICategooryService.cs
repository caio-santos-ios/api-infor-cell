using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ICategoryService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Category?>> CreateAsync(CreateCategoryDTO request);
    Task<ResponseApi<Category?>> UpdateAsync(UpdateCategoryDTO request);
    //Task<ResponseApi<Category?>> SavePhotoProfileAsync(SaveCategoryPhotoDTO request);
    Task<ResponseApi<Category>> DeleteAsync(string id);
}
}