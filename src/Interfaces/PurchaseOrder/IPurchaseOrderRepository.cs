using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IPurchaseOrderRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PurchaseOrder> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PurchaseOrder?>> GetByIdAsync(string id);
    Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<PurchaseOrder> pagination);
    Task<ResponseApi<PurchaseOrder?>> CreateAsync(PurchaseOrder address);
    Task<ResponseApi<PurchaseOrder?>> UpdateAsync(PurchaseOrder address);
    Task<ResponseApi<PurchaseOrder>> DeleteAsync(string id);
}
}
