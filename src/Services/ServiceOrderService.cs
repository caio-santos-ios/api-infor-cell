using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ServiceOrderService(IServiceOrderRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IServiceOrderService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<ServiceOrder> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> ServiceOrders = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(ServiceOrders.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> ServiceOrder = await repository.GetByIdAggregateAsync(id);
            if(ServiceOrder.Data is null) return new(null, 404, "Loja não encontrada");
            return new(ServiceOrder.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<ServiceOrder?>> CreateAsync(CreateServiceOrderDTO request)
    {
        try
        {
            ServiceOrder ServiceOrder = _mapper.Map<ServiceOrder>(request);
            ResponseApi<ServiceOrder?> response = await repository.CreateAsync(ServiceOrder);

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
    public async Task<ResponseApi<ServiceOrder?>> UpdateAsync(UpdateServiceOrderDTO request)
    {
        try
        {
            ResponseApi<ServiceOrder?> ServiceOrderResponse = await repository.GetByIdAsync(request.Id);
            if(ServiceOrderResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ServiceOrder ServiceOrder = _mapper.Map<ServiceOrder>(request);
            ServiceOrder.UpdatedAt = DateTime.UtcNow;

            ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(ServiceOrder);
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
    public async Task<ResponseApi<ServiceOrder>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<ServiceOrder> ServiceOrder = await repository.DeleteAsync(id);
            if(!ServiceOrder.IsSuccess) return new(null, 400, ServiceOrder.Message);
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