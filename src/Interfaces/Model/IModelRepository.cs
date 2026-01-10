using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IModelRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Model> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Model?>> GetByIdAsync(string id);
    Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Model> pagination);
    Task<ResponseApi<Model?>> CreateAsync(Model address);
    Task<ResponseApi<Model?>> UpdateAsync(Model address);
    Task<ResponseApi<Model>> DeleteAsync(string id);
}
}
