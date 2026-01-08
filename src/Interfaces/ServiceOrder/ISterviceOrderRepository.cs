using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IServiceOrderRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceOrder> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ServiceOrder?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceOrder> pagination);
    Task<ResponseApi<ServiceOrder?>> CreateAsync(ServiceOrder serviceOrder);
    Task<ResponseApi<ServiceOrder?>> UpdateAsync(ServiceOrder serviceOrder);
    Task<ResponseApi<ServiceOrder>> DeleteAsync(string id);
}
}
