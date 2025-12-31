namespace api_infor_cell.src.Shared.DTOs
{
    public class ResetPasswordDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string CodeAccess { get; set; } = string.Empty;
    }
}