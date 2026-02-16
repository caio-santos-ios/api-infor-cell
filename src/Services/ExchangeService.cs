using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ExchangeService(IExchangeRepository repository, IStockService stockService, ISalesOrderItemRepository salesOrderItemRepository, IMapper _mapper) : IExchangeService
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
            if(Exchange.Data is null) return new(null, 404, "Tranferência não encontrada");
            return new(Exchange.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetBySalesOrderItemIdAggregateAsync(string salesOrderItemId)
    {
        try
        {
            ResponseApi<List<dynamic>> Exchange = await repository.GetBySalesOrderItemIdAggregateAsync(salesOrderItemId);
            if(Exchange.Data is null) return new(null, 404, "Tranferência não encontrada");
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
            Exchange exchange = _mapper.Map<Exchange>(request);
            exchange.UpdatedAt = DateTime.UtcNow;
            exchange.ReleasedStock = false;

            ResponseApi<Exchange?> response = await repository.CreateAsync(exchange);
            if(!response.IsSuccess) return new(null, 400, "Falha ao salvar");

            ResponseApi<SalesOrderItem?> salesOrderItem = await salesOrderItemRepository.GetByIdAsync(request.SalesOrderItemId);
            if(salesOrderItem.Data is not null)
            {
                salesOrderItem.Data.UpdatedAt = DateTime.Now;
                salesOrderItem.Data.UpdatedBy = request.UpdatedBy;
                decimal total = salesOrderItem.Data.Total - request.Cost;
                salesOrderItem.Data.Total = total < 0 ? 0 : total;

                await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
            };

            return new(response.Data, 201, "Troca salva com sucesso!");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    public async Task<ResponseApi<Exchange?>> CreateReleasedStockAsync(CreateExchangeDTO request)
    {
        try
        {
            ResponseApi<List<Exchange>> exchanges = await repository.GetReleasedStockAsync(request.Plan, request.Company, request.Store);
            
            if(!exchanges.IsSuccess || exchanges.Data is null) return new(null, 400, "Falha ao salvar");
            foreach (Exchange exchange in exchanges.Data)
            {
                exchange.UpdatedAt = DateTime.UtcNow;
                exchange.UpdatedBy = request.UpdatedBy;
                exchange.ReleasedStock = true;
                
                await stockService.CreateAsync(new () 
                {
                    ProductId = exchange.ProductId,
                    Variations = exchange.Variations,
                    VariationsCode = exchange.VariationsCode,
                    Quantity = 1,
                    Origin = exchange.Origin,
                    OriginId = exchange.SalesOrderItemId,
                    ForSale = exchange.ForSale,
                    Cost = exchange.Cost,
                    Plan = exchange.Plan,
                    Company = exchange.Company,
                    Store = exchange.Store
                });
            }

            return new(null, 201, "Troca salva com sucesso!");
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
            ResponseApi<Exchange?> exchangeResponse = await repository.GetByIdAsync(request.Id);
            if(exchangeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            decimal oldCost = exchangeResponse.Data.Cost;

            Exchange exchange = _mapper.Map<Exchange>(request);
            exchange.UpdatedAt = DateTime.UtcNow;
            exchange.ReleasedStock = false;

            ResponseApi<Exchange?> response = await repository.UpdateAsync(exchange);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            ResponseApi<SalesOrderItem?> salesOrderItem = await salesOrderItemRepository.GetByIdAsync(request.SalesOrderItemId);
            if(salesOrderItem.Data is not null)
            {
                decimal total = salesOrderItem.Data.Total + oldCost;

                decimal newTotal = total - exchange.Cost;

                salesOrderItem.Data.Total = Math.Max(0, newTotal);
                
                salesOrderItem.Data.UpdatedAt = DateTime.Now;
                salesOrderItem.Data.UpdatedBy = request.UpdatedBy;

                await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
            };

            return new(response.Data, 201, "Atualizado com sucesso");
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
            ResponseApi<Exchange> exchange = await repository.DeleteAsync(id);
            if(!exchange.IsSuccess || exchange.Data is null) return new(null, 400, exchange.Message);

            ResponseApi<SalesOrderItem?> salesOrderItem = await salesOrderItemRepository.GetByIdAsync(exchange.Data.SalesOrderItemId);
            if(salesOrderItem.Data is not null)
            {
                salesOrderItem.Data.UpdatedAt = DateTime.Now;
                salesOrderItem.Data.Total = exchange.Data.Cost + salesOrderItem.Data.Total; 

                await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
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