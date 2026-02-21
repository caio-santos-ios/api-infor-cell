using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;

namespace api_infor_cell.src.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<ResponseApi<Subscription?>> GetByUserIdAsync(string userId);
        Task<ResponseApi<Subscription?>> GetByPlanIdAsync(string planId);
        Task<ResponseApi<Subscription?>> GetByAsaasSubscriptionIdAsync(string asaasSubscriptionId);
        Task<ResponseApi<Subscription?>> CreateAsync(Subscription subscription);
        Task<ResponseApi<Subscription?>> UpdateAsync(Subscription subscription);
    }
}