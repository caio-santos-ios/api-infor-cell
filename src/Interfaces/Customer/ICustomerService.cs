using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ICustomerService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request);
    Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request);
    //Task<ResponseApi<Customer?>> SavePhotoProfileAsync(SaveCustomerPhotoDTO request);
    Task<ResponseApi<Customer>> DeleteAsync(string id);
}
}