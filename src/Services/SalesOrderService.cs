using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class SalesOrderService(ISalesOrderRepository repository, IMapper _mapper) : ISalesOrderService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<SalesOrder> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> SalesOrders = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(SalesOrders.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> SalesOrder = await repository.GetByIdAggregateAsync(id);
            if(SalesOrder.Data is null) return new(null, 404, "Loja não encontrada");
            return new(SalesOrder.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SalesOrder?>> CreateAsync(CreateSalesOrderDTO request)
    {
        try
        {
            SalesOrder SalesOrder = _mapper.Map<SalesOrder>(request);
            ResponseApi<SalesOrder?> response = await repository.CreateAsync(SalesOrder);

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
    public async Task<ResponseApi<SalesOrder?>> UpdateAsync(UpdateSalesOrderDTO request)
    {
        try
        {
            ResponseApi<SalesOrder?> SalesOrderResponse = await repository.GetByIdAsync(request.Id);
            if(SalesOrderResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            SalesOrder SalesOrder = _mapper.Map<SalesOrder>(request);
            SalesOrder.UpdatedAt = DateTime.UtcNow;

            ResponseApi<SalesOrder?> response = await repository.UpdateAsync(SalesOrder);
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
    public async Task<ResponseApi<SalesOrder>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<SalesOrder> SalesOrder = await repository.DeleteAsync(id);
            if(!SalesOrder.IsSuccess) return new(null, 400, SalesOrder.Message);
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