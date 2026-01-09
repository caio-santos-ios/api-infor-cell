using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ServiceOrderItemService(IServiceOrderItemRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IServiceOrderItemService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<ServiceOrderItem> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> ServiceOrderItems = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(ServiceOrderItems.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> ServiceOrderItem = await repository.GetByIdAggregateAsync(id);
            if(ServiceOrderItem.Data is null) return new(null, 404, "Loja não encontrada");
            return new(ServiceOrderItem.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<ServiceOrderItem?>> CreateAsync(CreateServiceOrderItemDTO request)
    {
        try
        {
            ServiceOrderItem ServiceOrderItem = _mapper.Map<ServiceOrderItem>(request);
            ResponseApi<ServiceOrderItem?> response = await repository.CreateAsync(ServiceOrderItem);

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
    public async Task<ResponseApi<ServiceOrderItem?>> UpdateAsync(UpdateServiceOrderItemDTO request)
    {
        try
        {
            ResponseApi<ServiceOrderItem?> ServiceOrderItemResponse = await repository.GetByIdAsync(request.Id);
            if(ServiceOrderItemResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ServiceOrderItem ServiceOrderItem = _mapper.Map<ServiceOrderItem>(request);
            ServiceOrderItem.UpdatedAt = DateTime.UtcNow;

            ResponseApi<ServiceOrderItem?> response = await repository.UpdateAsync(ServiceOrderItem);
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
    public async Task<ResponseApi<ServiceOrderItem>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<ServiceOrderItem> ServiceOrderItem = await repository.DeleteAsync(id);
            if(!ServiceOrderItem.IsSuccess) return new(null, 400, ServiceOrderItem.Message);
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