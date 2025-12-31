using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IAddressService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Address?>> GetByParentIdAsync(string parentId, string parent);
    Task<ResponseApi<Address?>> CreateAsync(CreateAddressDTO request);
    Task<ResponseApi<Address?>> UpdateAsync(UpdateAddressDTO request);
    Task<ResponseApi<Address>> DeleteAsync(string id);
}
}