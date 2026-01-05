namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdatePlanDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; 
        public DateTime StartDate {get;set;} = DateTime.UtcNow;
    }
}