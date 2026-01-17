using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IStockRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Stock> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Stock?>> GetByIdAsync(string id);
    Task<ResponseApi<List<Stock>>> GetByPurchaseItemIdAsync(string purchaseOrderItemId, string planId, string companyId, string storeId);
    Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Stock> pagination);
    Task<ResponseApi<Stock?>> CreateAsync(Stock address);
    Task<ResponseApi<Stock?>> UpdateAsync(Stock address);
    Task<ResponseApi<Stock>> DeleteAsync(string id);
}
}
