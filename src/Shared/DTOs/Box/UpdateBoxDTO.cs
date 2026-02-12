namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateBoxDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public decimal OpeningValue {get; set;} = 0;
        public string TwoSteps {get; set;} = string.Empty;
    }
}
