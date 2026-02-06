using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IBoxRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Box> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<dynamic?>> GetByCreatedIdAggregateAsync(string createdBy);
    Task<ResponseApi<Box?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Box> pagination);
    Task<ResponseApi<Box?>> CreateAsync(Box address);
    Task<ResponseApi<Box?>> UpdateAsync(Box address);
    Task<ResponseApi<Box>> DeleteAsync(string id);
}
}
