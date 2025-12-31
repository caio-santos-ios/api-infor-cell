using api_infor_cell.src.Models;

namespace api_infor_cell.src.Shared.DTOs
{
    public class UpdateSupplierDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "F";
        public string Document { get; set; } = string.Empty;
        public string CorporateName { get; set; } = string.Empty;

        public string TradeName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public Address Address { get; set; } = new Address();

        public string StateRegistration { get; set; } = string.Empty;

        public string MunicipalRegistration { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
        public DateTime? EffectiveDate { get; set; }
    }
}