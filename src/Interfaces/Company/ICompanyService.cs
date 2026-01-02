using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ICompanyService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Company?>> CreateAsync(CreateCompanyDTO request);
    Task<ResponseApi<Company?>> UpdateAsync(UpdateCompanyDTO request);
    Task<ResponseApi<Company?>> SavePhotoProfileAsync(SaveCompanyPhotoDTO request);
    Task<ResponseApi<Company>> DeleteAsync(string id);
}
}