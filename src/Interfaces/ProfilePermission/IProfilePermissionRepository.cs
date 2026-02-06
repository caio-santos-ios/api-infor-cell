using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IProfilePermissionRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ProfilePermission> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ProfilePermission?>> GetByIdAsync(string id);
    Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ProfilePermission> pagination);
    Task<int> GetCountDocumentsAsync(PaginationUtil<ProfilePermission> pagination);
    Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId);
    Task<ResponseApi<ProfilePermission?>> CreateAsync(ProfilePermission address);
    Task<ResponseApi<ProfilePermission?>> UpdateAsync(ProfilePermission address);
    Task<ResponseApi<ProfilePermission>> DeleteAsync(string id);
}
}
