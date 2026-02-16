using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface ILogApiService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        // Task<ResponseApi<LogApi?>> CreateAsync(CreateLogApiDTO request);
    }
}