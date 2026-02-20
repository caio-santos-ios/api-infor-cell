namespace api_infor_cell.src.Shared.DTOs
{
    public class RegisterDTO
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool PrivacyPolicy { get; set; } = false;
    }
}