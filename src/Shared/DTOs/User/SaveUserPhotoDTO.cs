namespace api_infor_cell.src.Shared.DTOs
{
    public class SaveUserPhotoDTO
    {
        public string Id { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
    }
}