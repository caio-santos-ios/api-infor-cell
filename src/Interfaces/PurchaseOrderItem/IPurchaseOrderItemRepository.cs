using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IPurchaseOrderItemRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PurchaseOrderItem> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PurchaseOrderItem?>> GetByIdAsync(string id);
    Task<ResponseApi<List<PurchaseOrderItem>>> GetByPurchaseOrderIdAsync(string purchaseOrderId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<PurchaseOrderItem> pagination);
    Task<ResponseApi<PurchaseOrderItem?>> CreateAsync(PurchaseOrderItem address);
    Task<ResponseApi<PurchaseOrderItem?>> UpdateAsync(PurchaseOrderItem address);
    Task<ResponseApi<PurchaseOrderItem>> DeleteAsync(string id);
}
}
