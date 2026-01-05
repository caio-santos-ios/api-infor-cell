namespace api_infor_cell.src.Shared.DTOs
{
    public class CreatePlanDTO
    {
        public string Type { get; set; } = string.Empty; 
        public DateTime StartDate {get;set;} = DateTime.UtcNow;
    }
}