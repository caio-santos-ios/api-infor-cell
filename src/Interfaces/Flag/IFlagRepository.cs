using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IFlagRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Flag> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Flag?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Flag> pagination);
    Task<ResponseApi<Flag?>> CreateAsync(Flag address);
    Task<ResponseApi<Flag?>> UpdateAsync(Flag address);
    Task<ResponseApi<Flag>> DeleteAsync(string id);
}
}
