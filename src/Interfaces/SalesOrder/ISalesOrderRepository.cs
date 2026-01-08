using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ISalesOrderRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<SalesOrder> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<SalesOrder?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<SalesOrder> pagination);
    Task<ResponseApi<SalesOrder?>> CreateAsync(SalesOrder address);
    Task<ResponseApi<SalesOrder?>> UpdateAsync(SalesOrder address);
    Task<ResponseApi<SalesOrder>> DeleteAsync(string id);
}
}
