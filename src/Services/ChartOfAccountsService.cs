using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Services
{
    public class ChartOfAccountsService(IChartOfAccountsRepository repository) : IChartOfAccountsService
    {
        public async Task<ResponseApi<dynamic?>> GetAllAsync(Dictionary<string, string> queries, RequestDTO requestDTO)
        {
            try
            {
                PaginationUtil<ChartOfAccounts> pagination = new(queries);

                List<dynamic> list = (await repository.GetAllAsync(pagination)).Data ?? [];
                int count = await repository.GetCountDocumentsAsync(pagination);

                dynamic response = new
                {
                    data = list,
                    page = pagination.PageNumber,
                    pageSize = pagination.PageSize,
                    count
                };

                return new(response);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano de Contas");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAsync(string id)
        {
            try
            {
                dynamic? obj = (await repository.GetByIdAggregateAsync(id)).Data;
                return new(obj);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Conta");
            }
        }

        public async Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId)
        {
            try
            {
                long nextCode = (await repository.GetNextCodeAsync(planId, companyId)).Data;
                return new(nextCode);
            }
            catch
            {
                return new(1, 500, "Falha ao buscar próximo código");
            }
        }

        public async Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                chartOfAccounts.CreatedAt = DateTime.UtcNow;
                ResponseApi<ChartOfAccounts?> response = await repository.CreateAsync(chartOfAccounts);
                return new(response.Data, response.StatusCode, "Conta criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Conta");
            }
        }

        public async Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(string id, ChartOfAccounts chartOfAccounts)
        {
            try
            {
                ResponseApi<ChartOfAccounts?> existingAccount = await repository.GetByIdAsync(id);

                if (existingAccount.Data is null)
                {
                    return new(null, 404, "Conta não encontrada");
                }

                chartOfAccounts.Id = id;
                chartOfAccounts.UpdatedAt = DateTime.UtcNow;
                chartOfAccounts.CreatedAt = existingAccount.Data.CreatedAt;
                chartOfAccounts.CreatedBy = existingAccount.Data.CreatedBy;

                ResponseApi<ChartOfAccounts?> response = await repository.UpdateAsync(chartOfAccounts);
                return new(response.Data, response.StatusCode, "Conta atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Conta");
            }
        }

        public async Task<ResponseApi<ChartOfAccounts?>> DeleteAsync(string id, RequestDTO requestDTO)
        {
            try
            {
                ResponseApi<ChartOfAccounts?> existingAccount = await repository.GetByIdAsync(id);

                if (existingAccount.Data is null)
                {
                    return new(null, 404, "Conta não encontrada");
                }

                existingAccount.Data.DeletedAt = DateTime.UtcNow;
                existingAccount.Data.DeletedBy = requestDTO.DeletedBy;

                ResponseApi<ChartOfAccounts> response = await repository.DeleteAsync(id);
                return new(response.Data, response.StatusCode, "Conta excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluir Conta");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetTreeAsync(string planId, string companyId)
        {
            try
            {
                ResponseApi<List<dynamic>> response = await repository.GetTreeAsync(planId, companyId);
                return new(response.Data);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar árvore de contas");
            }
        }
    }
}