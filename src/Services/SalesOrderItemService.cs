using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class SalesOrderItemService(ISalesOrderItemRepository repository, IMapper _mapper) : ISalesOrderItemService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<SalesOrderItem> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> SalesOrderItems = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(SalesOrderItems.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> SalesOrderItem = await repository.GetByIdAggregateAsync(id);
            if(SalesOrderItem.Data is null) return new(null, 404, "Loja não encontrada");
            return new(SalesOrderItem.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SalesOrderItem?>> CreateAsync(CreateSalesOrderItemDTO request)
    {
        try
        {
            SalesOrderItem SalesOrderItem = _mapper.Map<SalesOrderItem>(request);
            ResponseApi<SalesOrderItem?> response = await repository.CreateAsync(SalesOrderItem);

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
    public async Task<ResponseApi<SalesOrderItem?>> UpdateAsync(UpdateSalesOrderItemDTO request)
    {
        try
        {
            ResponseApi<SalesOrderItem?> SalesOrderItemResponse = await repository.GetByIdAsync(request.Id);
            if(SalesOrderItemResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            SalesOrderItem SalesOrderItem = _mapper.Map<SalesOrderItem>(request);
            SalesOrderItem.UpdatedAt = DateTime.UtcNow;

            ResponseApi<SalesOrderItem?> response = await repository.UpdateAsync(SalesOrderItem);
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
    public async Task<ResponseApi<SalesOrderItem>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<SalesOrderItem> SalesOrderItem = await repository.DeleteAsync(id);
            if(!SalesOrderItem.IsSuccess) return new(null, 400, SalesOrderItem.Message);
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