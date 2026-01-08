using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IExchangeRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Exchange> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Exchange?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Exchange> pagination);
    Task<ResponseApi<Exchange?>> CreateAsync(Exchange address);
    Task<ResponseApi<Exchange?>> UpdateAsync(Exchange address);
    Task<ResponseApi<Exchange>> DeleteAsync(string id);
}
}
