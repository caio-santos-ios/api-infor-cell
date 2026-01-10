using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ExchangeService(IExchangeRepository repository, IMapper _mapper) : IExchangeService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Exchange> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Exchanges = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Exchanges.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Exchange = await repository.GetByIdAggregateAsync(id);
            if(Exchange.Data is null) return new(null, 404, "Loja não encontrada");
            return new(Exchange.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Exchange?>> CreateAsync(CreateExchangeDTO request)
    {
        try
        {
            Exchange Exchange = _mapper.Map<Exchange>(request);
            ResponseApi<Exchange?> response = await repository.CreateAsync(Exchange);

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
    public async Task<ResponseApi<Exchange?>> UpdateAsync(UpdateExchangeDTO request)
    {
        try
        {
            ResponseApi<Exchange?> ExchangeResponse = await repository.GetByIdAsync(request.Id);
            if(ExchangeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Exchange Exchange = _mapper.Map<Exchange>(request);
            Exchange.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Exchange?> response = await repository.UpdateAsync(Exchange);
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
    public async Task<ResponseApi<Exchange>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Exchange> Exchange = await repository.DeleteAsync(id);
            if(!Exchange.IsSuccess) return new(null, 400, Exchange.Message);
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