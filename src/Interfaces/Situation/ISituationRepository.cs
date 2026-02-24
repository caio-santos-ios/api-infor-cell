using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface ISituationRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Situation?>> GetByIdAsync(string id);
    Task<ResponseApi<List<Situation>>> GetByAppearsOnPanelAsync(string plan, string company, string store);
    Task<ResponseApi<Situation?>> GetByMomentAsync(string plan, string company, string store, string moment);
    Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Situation> pagination);
    Task<ResponseApi<Situation?>> CreateAsync(Situation situation);
    Task<ResponseApi<Situation?>> UpdateAsync(Situation situation);
    Task<ResponseApi<Situation>> DeleteAsync(string id);
}
}
