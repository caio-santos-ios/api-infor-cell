using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ISalesOrderItemRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<SalesOrderItem> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<SalesOrderItem?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<SalesOrderItem> pagination);
    Task<ResponseApi<SalesOrderItem?>> CreateAsync(SalesOrderItem address);
    Task<ResponseApi<SalesOrderItem?>> UpdateAsync(SalesOrderItem address);
    Task<ResponseApi<SalesOrderItem>> DeleteAsync(string id);
}
}
