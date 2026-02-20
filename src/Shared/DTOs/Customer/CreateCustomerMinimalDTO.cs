namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateCustomerMinimalDTO : RequestDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}