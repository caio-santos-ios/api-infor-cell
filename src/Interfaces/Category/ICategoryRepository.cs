using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ICategoryRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Category> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Category?>> GetByIdAsync(string id);
    Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Category> pagination);
    Task<ResponseApi<Category?>> CreateAsync(Category address);
    Task<ResponseApi<Category?>> UpdateAsync(Category address);
    Task<ResponseApi<Category>> DeleteAsync(string id);
}
}
