using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ExchangeService(IExchangeRepository repository, IStockService stockService, IStockRepository stockRepository, ISalesOrderItemRepository salesOrderItemRepository, ISalesOrderRepository salesOrderRepository, ICustomerRepository customerRepository, IAccountPayableService accountPayableService, IProductRepository productRepository, IUserRepository userRepository, IMapper _mapper) : IExchangeService
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
                ResponseApi<List<SalesOrderItem>> items = await salesOrderItemRepository.GetBySalesOrderIdAsync(salesOrderItem.Data.SalesOrderId, request.Plan, request.Company, request.Store);

                ResponseApi<SalesOrder?> salesOrder = await salesOrderRepository.GetByIdAsync(salesOrderItem.Data.SalesOrderId);

                if(salesOrder.Data is not null && request.Type == "return")
                {
                    int countItemsCanceleted = items.Data!.Where(x => x.Status == "Cancelado - Produto Devolvido").Count();

                    if(items.Data!.Count == countItemsCanceleted)
                    {
                        salesOrder.Data.Status = "Cancelado - Produto Devolvido";
                    }
                    else
                    {
                        if(items.Data.Count == 1)
                        {
                            salesOrder.Data.Status = "Cancelado - Produto Devolvido";
                        }
                    }

                    salesOrder.Data.UpdatedAt = DateTime.UtcNow;
                    salesOrder.Data.UpdatedBy = request.CreatedBy;

                    salesOrderItem.Data.UpdatedAt = DateTime.UtcNow;
                    salesOrderItem.Data.UpdatedBy = request.CreatedBy;
                    salesOrderItem.Data.Status = "Produto Devolvido";

                    await salesOrderRepository.UpdateAsync(salesOrder.Data);
                    await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
                    
                    ResponseApi<Product?> product = await productRepository.GetByIdAsync(salesOrderItem.Data.ProductId);
                    string productName = product.Data is not null ? product.Data.Name : "";

                    if(request.GenerateCashback)
                    {
                        ResponseApi<Customer?> customer = await customerRepository.GetByIdAsync(salesOrder.Data.CustomerId);
                        
                        if(customer.Data is not null)
                        {    
                            ResponseApi<User?> user = await userRepository.GetByIdAsync(request.CreatedBy);
                            string userName = user.Data is not null ? user.Data.Name : "";

                            customer.Data.UpdatedAt = DateTime.UtcNow;
                            customer.Data.UpdatedBy = request.CreatedBy;
                            customer.Data.Cashbacks.Add(new ()
                            {
                                Available = true,
                                Value = salesOrderItem.Data.Total,
                                CurrentValue = salesOrderItem.Data.Total,
                                Date = DateTime.UtcNow,
                                Description = $"Cashback gerado pelo devolução do produto {productName}",
                                Origin = "excharge",
                                OriginDescription = $"Devolução do produto {productName} - PDV nº {salesOrder.Data.Code}",
                                OriginId = exchange.Id,
                                Responsible = userName,
                            });

                            await customerRepository.UpdateAsync(customer.Data);
                        }
                    }
                    else
                    {
                        await accountPayableService.CreateAsync(new ()
                        {
                            Plan = request.Plan,
                            Company = request.Company,
                            Store = request.Store,
                            CreatedBy = request.CreatedBy,
                            Amount = salesOrderItem.Data.Value,
                            InstallmentNumber = 1,
                            TotalInstallments = 1,
                            Description = $"Reembolso na devolução do produto {productName} do PDV nº ${salesOrder.Data.Code}",
                            DueDate = DateTime.UtcNow,
                            IssueDate = DateTime.UtcNow,
                            OriginId = salesOrderItem.Data.Id,
                            OriginType = "sales-order",
                        });
                    }
                }

                if(request.Type == "return")
                {
                    foreach (string stockId in salesOrderItem.Data.StockIds)
                    {
                        ResponseApi<Stock?> stock = await stockRepository.GetByIdAsync(stockId);
                        if(stock.Data is not null)
                        {
                            if(request.HasVariations == "yes")
                            {

                            }
                            else
                            {
                                stock.Data.Quantity += salesOrderItem.Data.Quantity;
                                stock.Data.QuantityAvailable += salesOrderItem.Data.Quantity;
                                

                                await stockRepository.UpdateAsync(stock.Data);
                            }
                        }
                    }
                }
            };

            string message = request.Type == "return" ? "Devolução" : "Troca";
            return new(response.Data, 201, $"{message} salva com sucesso!");
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
                
                // await stockService.CreateAsync(new () 
                // {
                //     ProductId = exchange.ProductId,
                //     Variations = exchange.Variations,
                //     VariationsCode = exchange.VariationsCode,
                //     Quantity = 1,
                //     Origin = exchange.Origin,
                //     OriginId = exchange.SalesOrderItemId,
                //     ForSale = exchange.ForSale,
                //     Cost = exchange.Cost,
                //     Plan = exchange.Plan,
                //     Company = exchange.Company,
                //     Store = exchange.Store
                // });
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
            
            // decimal oldCost = exchangeResponse.Data.Cost;

            // Exchange exchange = _mapper.Map<Exchange>(request);
            // exchange.UpdatedAt = DateTime.UtcNow;
            // exchange.ReleasedStock = false;

            // ResponseApi<Exchange?> response = await repository.UpdateAsync(exchange);
            // if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            // ResponseApi<SalesOrderItem?> salesOrderItem = await salesOrderItemRepository.GetByIdAsync(request.SalesOrderItemId);
            // if(salesOrderItem.Data is not null)
            // {
            //     decimal total = salesOrderItem.Data.Total + oldCost;

            //     decimal newTotal = total - exchange.Cost;

            //     salesOrderItem.Data.Total = Math.Max(0, newTotal);
                
            //     salesOrderItem.Data.UpdatedAt = DateTime.Now;
            //     salesOrderItem.Data.UpdatedBy = request.UpdatedBy;

            //     await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
            // };

            return new(null, 201, "Atualizado com sucesso");
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
            // if(salesOrderItem.Data is not null)
            // {
            //     salesOrderItem.Data.UpdatedAt = DateTime.Now;
            //     salesOrderItem.Data.Total = exchange.Data.Cost + salesOrderItem.Data.Total; 

            //     await salesOrderItemRepository.UpdateAsync(salesOrderItem.Data);
            // };

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