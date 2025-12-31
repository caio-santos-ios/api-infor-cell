using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
    public interface IUserRepository
    {
        Task<ResponseApi<api_infor_cell.src.Models.User?>> CreateAsync(api_infor_cell.src.Models.User user);
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<api_infor_cell.src.Models.User> pagination);
        Task<ResponseApi<List<dynamic>>> GetSelectBarberAsync(PaginationUtil<api_infor_cell.src.Models.User> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> GetByIdAsync(string id);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> GetByUserNameAsync(string userName);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> GetByEmailAsync(string email);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> GetByPhoneAsync(string phone);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> GetByCodeAccessAsync(string codeAccess);
        Task<int> GetCountDocumentsAsync(PaginationUtil<api_infor_cell.src.Models.User> pagination);
        Task<bool> GetAccessValitedAsync(string codeAccess);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> UpdateCodeAccessAsync(string userId, string codeAccess);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> UpdateAsync(api_infor_cell.src.Models.User request);
        Task<ResponseApi<api_infor_cell.src.Models.User?>> ValidatedAccessAsync(string codeAccess);
        Task<ResponseApi<api_infor_cell.src.Models.User>> DeleteAsync(string id);
    }
}