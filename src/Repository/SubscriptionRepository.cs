using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using MongoDB.Driver;

namespace api_infor_cell.src.Repository
{
    public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
    {
        public async Task<ResponseApi<Subscription?>> GetByUserIdAsync(string userId)
        {
            try
            {
                Subscription? sub = await context.Subscriptions
                    .Find(x => x.UserId == userId && !x.Deleted)
                    .SortByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();
                return new(sub);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar assinatura");
            }
        }
        public async Task<ResponseApi<Subscription?>> GetByPlanIdAsync(string planId)
        {
            try
            {
                Subscription? sub = await context.Subscriptions
                    .Find(x => x.PlanId == planId && !x.Deleted)
                    .SortByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();
                return new(sub);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar assinatura");
            }
        }

        public async Task<ResponseApi<Subscription?>> GetByAsaasSubscriptionIdAsync(string asaasSubscriptionId)
        {
            try
            {
                Subscription? sub = await context.Subscriptions
                    .Find(x => x.AsaasSubscriptionId == asaasSubscriptionId && !x.Deleted)
                    .FirstOrDefaultAsync();
                return new(sub);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar assinatura");
            }
        }

        public async Task<ResponseApi<Subscription?>> CreateAsync(Subscription subscription)
        {
            try
            {
                await context.Subscriptions.InsertOneAsync(subscription);
                return new(subscription, 201, "Assinatura criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar assinatura");
            }
        }

        public async Task<ResponseApi<Subscription?>> UpdateAsync(Subscription subscription)
        {
            try
            {
                await context.Subscriptions.ReplaceOneAsync(x => x.Id == subscription.Id, subscription);
                return new(subscription, 200, "Assinatura atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar assinatura");
            }
        }
    }
}