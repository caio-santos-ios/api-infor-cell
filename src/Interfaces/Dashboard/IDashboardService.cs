using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseApi<dynamic?>> GetFirstCardAsync();
        Task<ResponseApi<List<dynamic>>> GetRecentPatientAsync();

    }
}