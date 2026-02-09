using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IStockRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Stock> pagination);
    Task<ResponseApi<List<dynamic>>> GetByProductIdAggregationAsync(string plan, string company, string productId);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Stock?>> GetByIdAsync(string id);
    Task<ResponseApi<Stock?>> GetByOriginIdAsync(string originId);
    Task<ResponseApi<List<Stock>>> GetStockTransfer(string productId, string barcode, string hasSerial, string serial, string planId, string companyId, string storeId);
    Task<ResponseApi<Stock?>> GetVerifyStock(string productId, string planId, string companyId, string storeId);
    Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Stock> pagination);
    Task<ResponseApi<Stock?>> CreateAsync(Stock stock);
    Task<ResponseApi<Stock?>> UpdateAsync(Stock stock);
    Task<ResponseApi<Stock>> DeleteAllByProductAsync(DeleteDTO request);
    Task<ResponseApi<Stock>> DeleteAsync(string id);
}
}
