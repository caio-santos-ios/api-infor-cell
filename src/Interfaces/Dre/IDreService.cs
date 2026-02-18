using api_infor_cell.src.Models.Base;

namespace api_infor_cell.src.Interfaces
{
    public interface IDreService
    {
        Task<ResponseApi<dynamic?>> GenerateAsync(
            string planId,
            string companyId,
            string storeId,
            DateTime startDate,
            DateTime endDate,
            string regime
        );
    }
}