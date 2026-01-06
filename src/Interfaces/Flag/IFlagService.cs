using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IFlagService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Flag?>> CreateAsync(CreateFlagDTO request);
    Task<ResponseApi<Flag?>> UpdateAsync(UpdateFlagDTO request);
    //Task<ResponseApi<Flag?>> SavePhotoProfileAsync(SaveFlagPhotoDTO request);
    Task<ResponseApi<Flag>> DeleteAsync(string id);
}
}