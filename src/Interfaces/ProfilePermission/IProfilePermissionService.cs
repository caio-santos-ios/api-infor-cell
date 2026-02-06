using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IProfilePermissionService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<ProfilePermission?>> CreateAsync(CreateProfilePermissionDTO request);
        Task<ResponseApi<ProfilePermission?>> UpdateAsync(UpdateProfilePermissionDTO request);
        Task<ResponseApi<ProfilePermission>> DeleteAsync(string id);
    }
}