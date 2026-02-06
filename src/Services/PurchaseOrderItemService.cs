using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class PurchaseOrderItemService(IPurchaseOrderItemRepository repository, IPurchaseOrderRepository purchaseOrderRepository, IMapper _mapper) : IPurchaseOrderItemService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<PurchaseOrderItem> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> PurchaseOrderItems = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(PurchaseOrderItems.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> PurchaseOrderItem = await repository.GetByIdAggregateAsync(id);
            if(PurchaseOrderItem.Data is null) return new(null, 404, "Item Pedido de Compra não encontrado");
            return new(PurchaseOrderItem.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<PurchaseOrderItem?>> CreateAsync(CreatePurchaseOrderItemDTO request)
    {
        try
        {
            PurchaseOrderItem purchaseOrderItem = _mapper.Map<PurchaseOrderItem>(request);
            purchaseOrderItem.Variations = request.Variations;

            ResponseApi<PurchaseOrderItem?> response = await repository.CreateAsync(purchaseOrderItem);

            ResponseApi<PurchaseOrder?> purchaseOrder = await purchaseOrderRepository.GetByIdAsync(purchaseOrderItem.PurchaseOrderId);
            if(purchaseOrder.Data is not null)
            {
                purchaseOrder.Data.UpdatedAt = DateTime.UtcNow;
                purchaseOrder.Data.UpdatedBy = request.UpdatedBy;
                purchaseOrder.Data.Quantity += request.Quantity;
                purchaseOrder.Data.Total += request.Cost * request.Quantity;

                await purchaseOrderRepository.UpdateAsync(purchaseOrder.Data);
            }

            if(response.Data is null) return new(null, 400, "Falha ao criar Item Pedido de Compra.");
            return new(response.Data, 201, "Item Pedido de Compra criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<PurchaseOrderItem?>> UpdateAsync(UpdatePurchaseOrderItemDTO request)
    {
        try
        {
            ResponseApi<PurchaseOrderItem?> PurchaseOrderItemResponse = await repository.GetByIdAsync(request.Id);
            if(PurchaseOrderItemResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            PurchaseOrderItem purchaseOrderItem = _mapper.Map<PurchaseOrderItem>(request);
            purchaseOrderItem.UpdatedAt = DateTime.UtcNow;
            purchaseOrderItem.UpdatedBy = request.UpdatedBy;

            ResponseApi<PurchaseOrderItem?> response = await repository.UpdateAsync(purchaseOrderItem);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");
            
            ResponseApi<PurchaseOrder?> purchaseOrder = await purchaseOrderRepository.GetByIdAsync(purchaseOrderItem.PurchaseOrderId);
            if(purchaseOrder.Data is not null)
            {
                ResponseApi<List<PurchaseOrderItem>> items = await repository.GetByPurchaseOrderIdAsync(response.Data.PurchaseOrderId);

                if(items.Data is not null) 
                {
                    decimal cost = items.Data.Sum(v => v.Cost);
                    decimal quantity = items.Data.Sum(q => q.Quantity);

                    purchaseOrder.Data.UpdatedAt = DateTime.UtcNow;
                    purchaseOrder.Data.UpdatedBy = request.UpdatedBy;
                    purchaseOrder.Data.Quantity = quantity;
                    purchaseOrder.Data.Total = cost * quantity;

                    await purchaseOrderRepository.UpdateAsync(purchaseOrder.Data);
                }
            }

            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<PurchaseOrderItem>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<PurchaseOrderItem> purchaseOrderItem = await repository.DeleteAsync(id);
            if(!purchaseOrderItem.IsSuccess || purchaseOrderItem.Data is null) return new(null, 400, purchaseOrderItem.Message);

            ResponseApi<PurchaseOrder?> purchaseOrder = await purchaseOrderRepository.GetByIdAsync(purchaseOrderItem.Data.PurchaseOrderId);
            if(purchaseOrder.Data is not null)
            {
                purchaseOrder.Data.UpdatedAt = DateTime.UtcNow;
                purchaseOrder.Data.Quantity -= purchaseOrderItem.Data.Quantity;
                purchaseOrder.Data.Total -= purchaseOrderItem.Data.Cost;

                await purchaseOrderRepository.UpdateAsync(purchaseOrder.Data);
            }

            return new(null, 204, "Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}