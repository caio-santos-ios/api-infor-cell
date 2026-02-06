using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class SalesOrderItemService(ISalesOrderItemRepository repository, ISalesOrderRepository salesOrderRepository, IMapper _mapper) : ISalesOrderItemService
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
            if(SalesOrderItem.Data is null) return new(null, 404, "Item do Pedido de Venda não encontrada");
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
            SalesOrderItem salesOrderItem = _mapper.Map<SalesOrderItem>(request);


            ResponseApi<SalesOrderItem?> response = await repository.CreateAsync(salesOrderItem);
            if(response.Data is null) return new(null, 400, "Falha ao criar Item do Pedido de Venda.");
            
            ResponseApi<SalesOrder?> salesOrder = await salesOrderRepository.GetByIdAsync(request.SalesOrderId);
            if(salesOrder.Data is not null)
            {
                // decimal total = items.Data.Sum(x => x.Total);
                // decimal quantity = items.Data.Sum(x => x.Quantity);
                // decimal discountValue = items.Data.Sum(x => x.DiscountValue);

                salesOrder.Data.Total = request.Total;
                salesOrder.Data.Quantity = request.Quantity;
                salesOrder.Data.DiscountValue = request.DiscountValue;
                salesOrder.Data.UpdatedAt = DateTime.UtcNow;
                salesOrder.Data.UpdatedBy = request.UpdatedBy;

                await salesOrderRepository.UpdateAsync(salesOrder.Data);
            };
            

            return new(response.Data, 201, "Item do Pedido de Venda criada com sucesso.");
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
            ResponseApi<SalesOrderItem?> salesOrderItemResponse = await repository.GetByIdAsync(request.Id);
            if(salesOrderItemResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            SalesOrderItem salesOrderItem = _mapper.Map<SalesOrderItem>(request);
            salesOrderItem.UpdatedAt = DateTime.UtcNow;
            salesOrderItem.CreatedAt = salesOrderItemResponse.Data.CreatedAt;

            ResponseApi<SalesOrderItem?> response = await repository.UpdateAsync(salesOrderItem);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            ResponseApi<List<SalesOrderItem>> items = await repository.GetBySalesOrderIdAsync(request.SalesOrderId, request.Plan, request.Company, request.Store);
            if(items.Data is not null)
            {
                ResponseApi<SalesOrder?> salesOrder = await salesOrderRepository.GetByIdAsync(request.SalesOrderId);

                if(salesOrder.Data is not null)
                {
                    decimal total = items.Data.Sum(x => x.Total);
                    decimal quantity = items.Data.Sum(x => x.Quantity);
                    decimal discountValue = items.Data.Sum(x => x.DiscountValue);

                    salesOrder.Data.Total = total;
                    salesOrder.Data.Quantity = quantity;
                    salesOrder.Data.DiscountValue = discountValue;
                    salesOrder.Data.UpdatedAt = DateTime.UtcNow;
                    salesOrder.Data.UpdatedBy = request.UpdatedBy;

                    await salesOrderRepository.UpdateAsync(salesOrder.Data);
                };
            };

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
            ResponseApi<SalesOrderItem> salesOrderItem = await repository.DeleteAsync(id);
            if(!salesOrderItem.IsSuccess || salesOrderItem.Data is null) return new(null, 400, salesOrderItem.Message);

            ResponseApi<List<SalesOrderItem>> items = await repository.GetBySalesOrderIdAsync(salesOrderItem.Data.SalesOrderId, salesOrderItem.Data.Plan, salesOrderItem.Data.Company, salesOrderItem.Data.Store);
            if(items.Data is not null)
            {
                ResponseApi<SalesOrder?> salesOrder = await salesOrderRepository.GetByIdAsync(salesOrderItem.Data.SalesOrderId);

                if(salesOrder.Data is not null)
                {
                    decimal total = items.Data.Sum(x => x.Total);
                    decimal quantity = items.Data.Sum(x => x.Quantity);
                    decimal discountValue = items.Data.Sum(x => x.DiscountValue);

                    salesOrder.Data.Total = total;
                    salesOrder.Data.Quantity = quantity;
                    salesOrder.Data.DiscountValue = discountValue;
                    salesOrder.Data.UpdatedAt = DateTime.UtcNow;

                    await salesOrderRepository.UpdateAsync(salesOrder.Data);
                };
            };

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