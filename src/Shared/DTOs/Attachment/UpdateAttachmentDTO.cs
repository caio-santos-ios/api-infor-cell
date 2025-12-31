namespace api_infor_cell.src.Shared.DTOs
{
public class UpdateAttachmentDTO
{
     public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
    public string ParentId { get; set; } = string.Empty;
    public string Parent { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
}