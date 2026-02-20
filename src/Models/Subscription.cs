using api_infor_cell.src.Models.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_infor_cell.src.Models
{
    public class Subscription : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        /// <summary>ID do usuário/empresa no sistema</summary>
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>ID do cliente no Asaas</summary>
        [BsonElement("asaasCustomerId")]
        public string AsaasCustomerId { get; set; } = string.Empty;

        /// <summary>ID da assinatura no Asaas</summary>
        [BsonElement("asaasSubscriptionId")]
        public string AsaasSubscriptionId { get; set; } = string.Empty;

        /// <summary>ID do último pagamento gerado pelo Asaas</summary>
        [BsonElement("asaasPaymentId")]
        public string AsaasPaymentId { get; set; } = string.Empty;

        /// <summary>Tipo do plano: Bronze, Prata, Ouro, Platina</summary>
        [BsonElement("planType")]
        public string PlanType { get; set; } = string.Empty;

        /// <summary>ID do plano no sistema</summary>
        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty;

        /// <summary>Forma de pagamento: PIX, BOLETO, CREDIT_CARD, DEBIT_CARD</summary>
        [BsonElement("billingType")]
        public string BillingType { get; set; } = string.Empty;

        /// <summary>Status: PENDING, ACTIVE, OVERDUE, CANCELLED</summary>
        [BsonElement("status")]
        public string Status { get; set; } = "PENDING";

        [BsonElement("value")]
        public decimal Value { get; set; }

        [BsonElement("nextDueDate")]
        public DateTime? NextDueDate { get; set; }

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [BsonElement("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>Link do boleto ou QR Code PIX para pagamento</summary>
        [BsonElement("paymentUrl")]
        public string PaymentUrl { get; set; } = string.Empty;

        /// <summary>Linha digitável do boleto</summary>
        [BsonElement("identificationField")]
        public string IdentificationField { get; set; } = string.Empty;

        /// <summary>Código PIX Copia e Cola</summary>
        [BsonElement("pixQrCode")]
        public string PixQrCode { get; set; } = string.Empty;

        /// <summary>Imagem base64 do QR Code PIX</summary>
        [BsonElement("pixQrCodeImage")]
        public string PixQrCodeImage { get; set; } = string.Empty;
    }
}