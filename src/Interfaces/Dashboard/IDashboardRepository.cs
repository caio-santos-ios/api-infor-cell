using api_infor_cell.src.Models.Base;

namespace api_infor_cell.src.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ResponseApi<dynamic>> GetCardsAsync(string plan, string company, string store);
        Task<ResponseApi<dynamic>> GetMonthlySalesAsync(string plan, string company, string store);
        Task<ResponseApi<dynamic>> GetMonthlyTargetAsync(string plan, string company, string store);
        Task<ResponseApi<dynamic>> GetRecentOrdersAsync(string plan, string company, string store);
    }
}