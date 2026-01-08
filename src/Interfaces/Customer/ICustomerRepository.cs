using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ICustomerRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Customer?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Customer> pagination);
    Task<ResponseApi<Customer?>> CreateAsync(Customer address);
    Task<ResponseApi<Customer?>> UpdateAsync(Customer address);
    Task<ResponseApi<Customer>> DeleteAsync(string id);
}
}
