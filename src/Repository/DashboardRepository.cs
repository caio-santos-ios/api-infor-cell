using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using MongoDB.Driver;

namespace api_infor_cell.src.Repository
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        public async Task<ResponseApi<dynamic>> GetFirstCardAsync()
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
                
                DateTime startOfYear = new(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime endOfYear = startOfYear.AddYears(1).AddTicks(-1);

                DateTime previousStartDate = startOfMonth.AddMonths(-1);
                DateTime previousEndDate = endOfMonth.AddMonths(-1);
                
                DateTime previousStartOfYear = startOfYear.AddYears(-1);
                DateTime previousEndOfYear = endOfYear.AddYears(-1);

                dynamic obj = new { };

                return new(obj);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."); ;
            }
        }
    }
}