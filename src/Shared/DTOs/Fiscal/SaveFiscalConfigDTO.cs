namespace api_infor_cell.src.Shared.DTOs
{
    public class SaveFiscalConfigDTO : RequestDTO
    {
        public string Environment { get; set; } = "homologacao";
        public int SeriesNfe { get; set; } = 1;
        public int SeriesNfce { get; set; } = 1;
        public string CertificateBase64 { get; set; } = string.Empty;
        public string CertificatePassword { get; set; } = string.Empty;
        public string Csc { get; set; } = string.Empty;
        public string CscId { get; set; } = string.Empty;
        public string DefaultCfopInState { get; set; } = "5102";
        public string DefaultCfopOutState { get; set; } = "6102";
        public string DefaultCfopService { get; set; } = "5933";
        public int TaxRegime { get; set; } = 1;
        public string Street { get; set; } = string.Empty;
        public string AddressNumber { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }
}