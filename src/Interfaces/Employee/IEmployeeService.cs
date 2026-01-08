using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IEmployeeService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Employee?>> CreateAsync(CreateEmployeeDTO request);
    Task<ResponseApi<Employee?>> UpdateAsync(UpdateEmployeeDTO request);
    //Task<ResponseApi<Employee?>> SavePhotoProfileAsync(SaveEmployeePhotoDTO request);
    Task<ResponseApi<Employee>> DeleteAsync(string id);
}
}