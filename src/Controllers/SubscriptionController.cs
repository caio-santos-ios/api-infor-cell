using System.Security.Claims;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/subscriptions")]
    [ApiController]
    public class SubscriptionController(ISubscriptionService service) : ControllerBase
    {
        /// <summary>
        /// Assina um plano. Autenticado.
        /// Body: { planType, billingType, cardHolderName?, cardNumber?, cardExpiryMonth?, cardExpiryYear?, cardCvv? }
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDTO body)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            ResponseApi<Subscription?> response = await service.CreateSubscriptionAsync(body, userId);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Busca a assinatura ativa do usuário logado</summary>
        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            ResponseApi<Subscription?> response = await service.GetCurrentSubscriptionAsync(userId);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Cancela a assinatura do usuário logado</summary>
        [Authorize]
        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            ResponseApi<Subscription?> response = await service.CancelSubscriptionAsync(userId);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>
        /// Webhook do Asaas — NÃO requer autenticação.
        /// Configure a URL no painel do Asaas: POST https://sua-api.com/api/subscriptions/webhook
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] AsaasWebhookDTO body)
        {
            var token = Request.Headers["asaas-access-token"].ToString();
            // if (token != _configuration["Asaas:WebhookToken"]) return Unauthorized();

            ResponseApi<string> response = await service.HandleWebhookAsync(body);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}