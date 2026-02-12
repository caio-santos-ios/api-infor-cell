using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ExchangeService(IExchangeRepository repository, IStockService stockService, IMapper _mapper) : IExchangeService
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

            // await stockService.CreateAsync(new () 
            // {
            //     ProductId = request.ProductId,
            //     Variations = request.Variations,
            //     VariationsCode = request.VariationsCode,
            //     Quantity = 1,
            //     Origin = request.Origin,
            //     OriginId = request.SalesOrderItemId,
            //     ForSale = request.ForSale,
            //     Cost = request.Cost,
            //     Plan = request.Plan,
            //     Company = request.Company,
            //     Store = request.Store
            // });

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
            ResponseApi<Exchange?> ExchangeResponse = await repository.GetByIdAsync(request.Id);
            if(ExchangeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Exchange Exchange = _mapper.Map<Exchange>(request);
            Exchange.UpdatedAt = DateTime.UtcNow;
            Exchange.ReleasedStock = false;

            ResponseApi<Exchange?> response = await repository.UpdateAsync(Exchange);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            // ResponseApi<Stock?> stock = await stockRepository.GetByOriginIdAsync(request.SalesOrderItemId);

            // if(!stock.IsSuccess || stock.Data is null) return new(null, 400, "Falha ao atualizar");

            // stock.Data.ProductId = request.ProductId;
            // stock.Data.Variations = request.Variations;
            // stock.Data.VariationsCode = request.VariationsCode;
            // stock.Data.Quantity = 1;
            // stock.Data.Origin = request.Origin;
            // stock.Data.OriginId = request.SalesOrderItemId;
            // stock.Data.ForSale = request.ForSale;
            // stock.Data.Cost = request.Cost;
            // stock.Data.Plan = request.Plan;
            // stock.Data.Company = request.Company;
            // stock.Data.Store = request.Store;         
            // stock.Data.UpdatedAt = DateTime.UtcNow;
            // stock.Data.UpdatedBy = request.UpdatedBy;

            // await stockRepository.UpdateAsync(stock.Data);

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