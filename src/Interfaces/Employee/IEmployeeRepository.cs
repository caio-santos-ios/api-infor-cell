using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IEmployeeRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Employee> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Employee?>> GetByIdAsync(string id);
    Task<ResponseApi<Employee?>> GetByUserIdAsync(string userId);
    // Task<ResponseApi<Employee?>> GetByEmailAsync(string email, string id);
    // Task<ResponseApi<Employee?>> GetByCpfAsync(string cpf, string id);
    // Task<ResponseApi<Employee?>> GetByCodeAccessAsync(string codeAccess);
    Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
    Task<ResponseApi<List<User>>> GetSellersAsync(string planId, string companyId, string storeId);
    Task<ResponseApi<List<User>>> GetTechniciansAsync(string planId, string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Employee> pagination);
    Task<ResponseApi<Employee?>> CreateAsync(Employee address);
    Task<ResponseApi<Employee?>> UpdateAsync(Employee address);
    Task<ResponseApi<Employee>> DeleteAsync(string id);
}
}
