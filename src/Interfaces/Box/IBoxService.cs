using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IBoxService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<dynamic?>> GetByCreatedIdAggregateAsync(string createdBy);
    Task<ResponseApi<Box?>> CreateAsync(CreateBoxDTO request);
    Task<ResponseApi<Box?>> UpdateAsync(UpdateBoxDTO request);
    Task<ResponseApi<Box>> DeleteAsync(string id);
}
}