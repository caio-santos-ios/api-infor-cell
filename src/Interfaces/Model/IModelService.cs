using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IModelService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Model?>> CreateAsync(CreateModelDTO request);
    Task<ResponseApi<Model?>> UpdateAsync(UpdateModelDTO request);
    //Task<ResponseApi<Model?>> SavePhotoProfileAsync(SaveModelPhotoDTO request);
    Task<ResponseApi<Model>> DeleteAsync(string id);
}
}