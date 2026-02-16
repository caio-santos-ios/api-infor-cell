using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface ILogApiRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<LogApi> pagination);
        Task<ResponseApi<LogApi?>> CreateAsync(LogApi logApi);
    }
}
