using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface ISubscriptionService
    {
        Task<ResponseApi<Subscription?>> CreateSubscriptionAsync(CreateSubscriptionDTO request, string userId);
        Task<ResponseApi<Subscription?>> GetCurrentSubscriptionAsync(string userId);
        Task<ResponseApi<Subscription?>> CancelSubscriptionAsync(string userId);
        Task<ResponseApi<string>> HandleWebhookAsync(AsaasWebhookDTO webhook);
    }
}