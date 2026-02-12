namespace api_infor_cell.src.Shared.DTOs
{
    public class CreateBoxDTO : RequestDTO
    {
        public decimal OpeningValue {get; set;} = 0;
        public string TwoSteps {get; set;} = string.Empty;
    }
}