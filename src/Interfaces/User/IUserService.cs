using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IUserService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request, string userId);
        Task<PaginationApi<List<dynamic>>> GetEmployeeAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetEmployeeByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetLoggedAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetSelectBarberAsync(GetAllDTO request);
        Task<ResponseApi<User?>> CreateAsync(CreateUserDTO user);
        Task<ResponseApi<User?>> CreateEmployeeAsync(CreateUserEmployeeDTO user);
        Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO user);
        Task<ResponseApi<User?>> UpdateModuleAsync(UpdateUserDTO user);
        Task<ResponseApi<User?>> UpdateStoreAsync(UpdateUserDTO user);
        Task<ResponseApi<User?>> SavePhotoProfileAsync(SaveUserPhotoDTO user);
        Task<ResponseApi<User?>> ResendCodeAccessAsync(UpdateUserDTO user);
        Task<ResponseApi<User?>> RemovePhotoProfileAsync(string id);
        Task<ResponseApi<User?>> ValidatedAccessAsync(string codeAccess);
        Task<ResponseApi<User>> DeleteAsync(string id);
    }
}