using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IServiceOrderItemRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ServiceOrderItem> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ServiceOrderItem?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<ServiceOrderItem> pagination);
    Task<ResponseApi<ServiceOrderItem?>> CreateAsync(ServiceOrderItem address);
    Task<ResponseApi<ServiceOrderItem?>> UpdateAsync(ServiceOrderItem address);
    Task<ResponseApi<ServiceOrderItem>> DeleteAsync(string id);
}
}
