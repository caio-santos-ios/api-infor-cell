using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IProductRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Product> pagination);
    Task<ResponseApi<List<dynamic>>> GetAutocompleteAsync(PaginationUtil<Product> pagination);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Product> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Product?>> GetByIdAsync(string id);
    Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Product> pagination);
    Task<ResponseApi<Product?>> CreateAsync(Product address);
    Task<ResponseApi<Product?>> UpdateAsync(Product address);
    Task<ResponseApi<Product>> DeleteAsync(string id);
}
}
