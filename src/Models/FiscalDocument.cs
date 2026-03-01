using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    /// <summary>
    /// Documento fiscal (NF-e modelo 55 ou NFC-e modelo 65).
    /// Segue state machine: Draft → Pending → Processing → Authorized | Rejected | Denied | Cancelled | Contingency
    /// </summary>
    public class FiscalDocument : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        // ─── Origem ──────────────────────────────────────────────────────────────
        /// <summary>ID da venda ou OS de origem</summary>
        [BsonElement("originId")]
        public string OriginId { get; set; } = string.Empty;

        /// <summary>sales_order | service_order</summary>
        [BsonElement("originType")]
        public string OriginType { get; set; } = string.Empty;

        // ─── Identificação fiscal ─────────────────────────────────────────────────
        /// <summary>55 = NF-e | 65 = NFC-e</summary>
        [BsonElement("model")]
        public int Model { get; set; } = 65;

        [BsonElement("series")]
        public int Series { get; set; } = 1;

        [BsonElement("number")]
        public long Number { get; set; }

        /// <summary>44 dígitos - chave de acesso SEFAZ</summary>
        [BsonElement("accessKey")]
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>Draft | Pending | Processing | Authorized | Rejected | Denied | Cancelled | Contingency</summary>
        [BsonElement("status")]
        public string Status { get; set; } = "Draft";

        /// <summary>homologacao | producao</summary>
        [BsonElement("environment")]
        public string Environment { get; set; } = "homologacao";

        // ─── Emitente (snapshot da loja no momento da emissão) ───────────────────
        [BsonElement("issuer")]
        public FiscalIssuer Issuer { get; set; } = new();

        // ─── Destinatário ─────────────────────────────────────────────────────────
        [BsonElement("recipient")]
        public FiscalRecipient Recipient { get; set; } = new();

        // ─── Itens ────────────────────────────────────────────────────────────────
        [BsonElement("items")]
        public List<FiscalDocumentItem> Items { get; set; } = [];

        // ─── Totais ───────────────────────────────────────────────────────────────
        [BsonElement("totals")]
        public FiscalTotals Totals { get; set; } = new();

        // ─── Pagamento ────────────────────────────────────────────────────────────
        [BsonElement("paymentMethodId")]
        public string PaymentMethodId { get; set; } = string.Empty;

        [BsonElement("paymentMethodName")]
        public string PaymentMethodName { get; set; } = string.Empty;

        // ─── XML e protocolo ──────────────────────────────────────────────────────
        [BsonElement("xmlSent")]
        public string XmlSent { get; set; } = string.Empty;

        [BsonElement("xmlAuthorized")]
        public string XmlAuthorized { get; set; } = string.Empty;

        [BsonElement("protocol")]
        public string Protocol { get; set; } = string.Empty;

        [BsonElement("receiptNumber")]
        public string ReceiptNumber { get; set; } = string.Empty;

        /// <summary>URL do DANFE em PDF (Cloudinary ou storage próprio)</summary>
        [BsonElement("danfeUrl")]
        public string DanfeUrl { get; set; } = string.Empty;

        // ─── Retornos da SEFAZ ────────────────────────────────────────────────────
        [BsonElement("sefazReturnCode")]
        public string SefazReturnCode { get; set; } = string.Empty;

        [BsonElement("sefazReturnMessage")]
        public string SefazReturnMessage { get; set; } = string.Empty;

        [BsonElement("attempts")]
        public List<FiscalAttempt> Attempts { get; set; } = [];

        // ─── Datas ────────────────────────────────────────────────────────────────
        [BsonElement("issuedAt")]
        public DateTime? IssuedAt { get; set; }

        [BsonElement("authorizedAt")]
        public DateTime? AuthorizedAt { get; set; }

        // ─── Contingência ─────────────────────────────────────────────────────────
        [BsonElement("isContingency")]
        public bool IsContingency { get; set; } = false;

        [BsonElement("contingencyReason")]
        public string ContingencyReason { get; set; } = string.Empty;

        [BsonElement("contingencyStartedAt")]
        public DateTime? ContingencyStartedAt { get; set; }

        // ─── Info complementar ────────────────────────────────────────────────────
        [BsonElement("additionalInfo")]
        public string AdditionalInfo { get; set; } = string.Empty;
    }

    public class FiscalIssuer
    {
        [BsonElement("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;

        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;

        [BsonElement("stateRegistration")]
        public string StateRegistration { get; set; } = string.Empty;

        [BsonElement("municipalRegistration")]
        public string MunicipalRegistration { get; set; } = string.Empty;

        /// <summary>1=Simples Nacional, 2=Simples Nacional excesso, 3=Regime Normal</summary>
        [BsonElement("taxRegime")]
        public int TaxRegime { get; set; } = 1;

        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;

        [BsonElement("district")]
        public string District { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("cityCode")]
        public string CityCode { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;
    }

    public class FiscalRecipient
    {
        [BsonElement("document")]
        public string Document { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;

        [BsonElement("district")]
        public string District { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("cityCode")]
        public string CityCode { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;

        /// <summary>9 = Não contribuinte</summary>
        [BsonElement("ieIndicator")]
        public int IeIndicator { get; set; } = 9;

        [BsonElement("stateRegistration")]
        public string StateRegistration { get; set; } = string.Empty;
    }

    public class FiscalDocumentItem
    {
        [BsonElement("itemNumber")]
        public int ItemNumber { get; set; }

        [BsonElement("productId")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("ncm")]
        public string Ncm { get; set; } = string.Empty;

        [BsonElement("cest")]
        public string Cest { get; set; } = string.Empty;

        [BsonElement("cfop")]
        public string Cfop { get; set; } = string.Empty;

        [BsonElement("unitOfMeasure")]
        public string UnitOfMeasure { get; set; } = "UN";

        [BsonElement("quantity")]
        public decimal Quantity { get; set; }

        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }

        [BsonElement("discount")]
        public decimal Discount { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        // ─── ICMS (Simples Nacional - CSOSN) ─────────────────────────────────────
        [BsonElement("csosn")]
        public string Csosn { get; set; } = "400"; // 400 = Tributado sem permissão de crédito (mais comum no Simples)

        [BsonElement("icmsOrigin")]
        public string IcmsOrigin { get; set; } = "0"; // 0 = Nacional

        // ─── PIS ─────────────────────────────────────────────────────────────────
        [BsonElement("pisCst")]
        public string PisCst { get; set; } = "07"; // 07 = Operação Isenta (Simples)

        [BsonElement("pisBase")]
        public decimal PisBase { get; set; }

        [BsonElement("pisRate")]
        public decimal PisRate { get; set; }

        [BsonElement("pisValue")]
        public decimal PisValue { get; set; }

        // ─── COFINS ───────────────────────────────────────────────────────────────
        [BsonElement("cofinsCst")]
        public string CofinsCst { get; set; } = "07";

        [BsonElement("cofinsBase")]
        public decimal CofinsBase { get; set; }

        [BsonElement("cofinsRate")]
        public decimal CofinsRate { get; set; }

        [BsonElement("cofinsValue")]
        public decimal CofinsValue { get; set; }

        /// <summary>service | product — para OS com misto</summary>
        [BsonElement("itemType")]
        public string ItemType { get; set; } = "product";

        [BsonElement("serial")]
        public string Serial { get; set; } = string.Empty;
    }

    public class FiscalTotals
    {
        [BsonElement("productsTotal")]
        public decimal ProductsTotal { get; set; }

        [BsonElement("discount")]
        public decimal Discount { get; set; }

        [BsonElement("freight")]
        public decimal Freight { get; set; }

        [BsonElement("grandTotal")]
        public decimal GrandTotal { get; set; }

        [BsonElement("icmsBase")]
        public decimal IcmsBase { get; set; }

        [BsonElement("icmsValue")]
        public decimal IcmsValue { get; set; }

        [BsonElement("pisValue")]
        public decimal PisValue { get; set; }

        [BsonElement("cofinsValue")]
        public decimal CofinsValue { get; set; }
    }

    public class FiscalAttempt
    {
        [BsonElement("attemptedAt")]
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("returnCode")]
        public string ReturnCode { get; set; } = string.Empty;

        [BsonElement("returnMessage")]
        public string ReturnMessage { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;
    }

    // ─── Eventos fiscais ──────────────────────────────────────────────────────────
    public class FiscalEvent : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("fiscalDocumentId")]
        public string FiscalDocumentId { get; set; } = string.Empty;

        [BsonElement("accessKey")]
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>Cancellation | CorrectionLetter | Numbering</summary>
        [BsonElement("eventType")]
        public string EventType { get; set; } = string.Empty;

        [BsonElement("reason")]
        public string Reason { get; set; } = string.Empty;

        [BsonElement("protocol")]
        public string Protocol { get; set; } = string.Empty;

        [BsonElement("xmlEvent")]
        public string XmlEvent { get; set; } = string.Empty;

        /// <summary>Pending | Authorized | Rejected</summary>
        [BsonElement("status")]
        public string Status { get; set; } = "Pending";

        [BsonElement("requestedByUserId")]
        public string RequestedByUserId { get; set; } = string.Empty;

        [BsonElement("processedAt")]
        public DateTime? ProcessedAt { get; set; }
    }

    // ─── Configuração fiscal por loja ─────────────────────────────────────────────
    public class FiscalConfig : ModelMasterBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>homologacao | producao</summary>
        [BsonElement("environment")]
        public string Environment { get; set; } = "homologacao";

        [BsonElement("seriesNfe")]
        public int SeriesNfe { get; set; } = 1;

        [BsonElement("seriesNfce")]
        public int SeriesNfce { get; set; } = 1;

        [BsonElement("nextNumberNfe")]
        public long NextNumberNfe { get; set; } = 1;

        [BsonElement("nextNumberNfce")]
        public long NextNumberNfce { get; set; } = 1;

        /// <summary>Certificado A1 (.pfx) em base64 — armazenar criptografado</summary>
        [BsonElement("certificateBase64")]
        public string CertificateBase64 { get; set; } = string.Empty;

        [BsonElement("certificatePassword")]
        public string CertificatePassword { get; set; } = string.Empty;

        [BsonElement("certificateExpiration")]
        public DateTime? CertificateExpiration { get; set; }

        /// <summary>CSC para QR Code NFC-e</summary>
        [BsonElement("csc")]
        public string Csc { get; set; } = string.Empty;

        [BsonElement("cscId")]
        public string CscId { get; set; } = string.Empty;

        /// <summary>CFOP padrão para venda dentro do estado</summary>
        [BsonElement("defaultCfopInState")]
        public string DefaultCfopInState { get; set; } = "5102";

        /// <summary>CFOP padrão para venda interestadual</summary>
        [BsonElement("defaultCfopOutState")]
        public string DefaultCfopOutState { get; set; } = "6102";

        /// <summary>CFOP padrão para serviços na OS</summary>
        [BsonElement("defaultCfopService")]
        public string DefaultCfopService { get; set; } = "5933";

        [BsonElement("taxRegime")]
        public int TaxRegime { get; set; } = 1;

        // Endereço do emitente (espelhado da loja, mas editável pelo contador)
        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("addressNumber")]
        public string AddressNumber { get; set; } = string.Empty;

        [BsonElement("district")]
        public string District { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("cityCode")]
        public string CityCode { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;
    }
}