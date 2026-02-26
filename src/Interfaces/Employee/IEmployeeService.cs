using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IEmployeeService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<User>>> GetSellersAsync(string planId, string companyId, string storeId);
        Task<ResponseApi<List<User>>> GetTechniciansAsync(string planId, string companyId, string storeId);
        // Task<ResponseApi<Employee?>> CreateAsync(CreateEmployeeDTO request);
        Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO request);
        Task<ResponseApi<User?>> UpdateModuleAsync(UpdateModuleEmployeeDTO request);
        Task<ResponseApi<User?>> UpdateCalendarAsync(UpdateCalendarEmployeeDTO request);
        Task<ResponseApi<User>> DeleteAsync(string id);
    }
}