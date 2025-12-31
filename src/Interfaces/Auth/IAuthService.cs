using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Responses;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseApi<AuthResponse>> LoginAsync(LoginDTO request);
        Task<ResponseApi<AuthResponse>> RefreshTokenAsync(string token);
        Task<ResponseApi<api_infor_cell.src.Models.User>> ResetPasswordAsync(ResetPasswordDTO request);
        Task<ResponseApi<api_infor_cell.src.Models.User>> RequestForgotPasswordAsync(ForgotPasswordDTO request);
        Task<ResponseApi<api_infor_cell.src.Models.User>> ForgotPasswordAsync(ResetPasswordDTO request);
    }
}