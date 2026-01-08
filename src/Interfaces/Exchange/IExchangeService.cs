using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IExchangeService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Exchange?>> CreateAsync(CreateExchangeDTO request);
    Task<ResponseApi<Exchange?>> UpdateAsync(UpdateExchangeDTO request);
    //Task<ResponseApi<Exchange?>> SavePhotoProfileAsync(SaveExchangePhotoDTO request);
    Task<ResponseApi<Exchange>> DeleteAsync(string id);
}
}