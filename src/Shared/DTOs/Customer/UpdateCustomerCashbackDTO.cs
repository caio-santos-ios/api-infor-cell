using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateCustomerCashbackDTO : RequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public List<CashbackCustomer> Cashbacks {get;set;} = [];
        public string ProductId { get; set; } = string.Empty;
    }
}
