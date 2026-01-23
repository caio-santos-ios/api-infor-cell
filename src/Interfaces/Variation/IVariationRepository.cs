using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IVariationRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Variation> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Variation?>> GetByIdAsync(string id);
        Task<ResponseApi<List<Variation>>> GetSerialExistedAsync(string serial);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Variation> pagination);
        Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Variation> pagination);
        Task<ResponseApi<Variation?>> CreateAsync(Variation variation);
        Task<ResponseApi<Variation?>> UpdateAsync(Variation variation);
        Task<ResponseApi<Variation>> DeleteAsync(string id);
    }
}