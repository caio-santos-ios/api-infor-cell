using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IBrandRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Brand> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Brand?>> GetByIdAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Brand> pagination);
    Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Brand> pagination);
    Task<ResponseApi<Brand?>> CreateAsync(Brand address);
    Task<ResponseApi<Brand?>> UpdateAsync(Brand address);
    Task<ResponseApi<Brand>> DeleteAsync(string id);
}
}
