using api_infor_cell.src.Models.Base;

namespace api_infor_cell.src.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ResponseApi<dynamic>> GetFirstCardAsync();
    }
}