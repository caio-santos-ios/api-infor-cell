using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class StoreService(IStoreRepository repository, IMapper _mapper) : IStoreService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Store> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Stores = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Stores.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> Store = await repository.GetByIdAggregateAsync(id);
            if(Store.Data is null) return new(null, 404, "Loja não encontrada");
            return new(Store.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Store> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> stores = await repository.GetSelectAsync(pagination);
            return new(stores.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }  
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Store?>> CreateAsync(CreateStoreDTO request)
    {
        try
        {
            Store Store = _mapper.Map<Store>(request);
            ResponseApi<Store?> response = await repository.CreateAsync(Store);

            if(response.Data is null) return new(null, 400, "Falha ao criar Loja.");
            return new(response.Data, 201, "Loja criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Store?>> UpdateAsync(UpdateStoreDTO request)
    {
        try
        {
            ResponseApi<Store?> StoreResponse = await repository.GetByIdAsync(request.Id);
            if(StoreResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Store Store = _mapper.Map<Store>(request);
            Store.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Store?> response = await repository.UpdateAsync(Store);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
   
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Store>> DeleteAsync(string id, string plan, string company)
    {
        try
        {
            ResponseApi<List<Store>> stores = await repository.GetTotalCompanies(plan, company);
            if(stores.Data is null) return new(null, 400, "O sistema deve possuir ao menos uma loja cadastrada.");
            if(stores.Data.Count == 1) return new(null, 400, "O sistema deve possuir ao menos uma loja cadastrada.");

            ResponseApi<Store> Store = await repository.DeleteAsync(id);
            if(!Store.IsSuccess) return new(null, 400, Store.Message);
            return new(null, 204, "Excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}