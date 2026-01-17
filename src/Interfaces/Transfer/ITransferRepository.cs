using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ITransferRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Transfer> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Transfer?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Transfer> pagination);
    Task<ResponseApi<Transfer?>> CreateAsync(Transfer transfer);
    Task<ResponseApi<Transfer?>> UpdateAsync(Transfer transfer);
    Task<ResponseApi<Transfer>> DeleteAsync(string id);
}
}
