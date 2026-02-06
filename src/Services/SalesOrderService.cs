using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class SalesOrderService(ISalesOrderRepository repository, ISalesOrderItemRepository salesOrderItemRepository, IStockRepository stockRepository, IProductRepository productRepository, IExchangeService exchangeService, IMapper _mapper) : ISalesOrderService
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
                if(SalesOrder.Data is null) return new(null, 404, "Pedido de Venda não encontrada");
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
                if(request.CustomerId.ToLower().Equals("ao consumidor"))
                {
                    request.CustomerId = "";
                };

                ResponseApi<long> code = await repository.GetNextCodeAsync(request.Plan, request.Company, request.Store);
                
                SalesOrder salesOrder = _mapper.Map<SalesOrder>(request);
                salesOrder.Status = "Rascunho";
                salesOrder.Code = code.Data.ToString().PadLeft(6, '0');

                ResponseApi<SalesOrder?> response = await repository.CreateAsync(salesOrder);

                if(response.Data is null) return new(null, 400, "Falha ao criar Pedido de Venda.");
                
                if(request.CreateItem)
                {
                    await salesOrderItemRepository.CreateAsync(new ()
                    {
                        Plan = request.Plan,
                        Company = request.Company,
                        Store = request.Store,
                        DiscountType = request.DiscountType,
                        DiscountValue = request.DiscountValue,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        Value = request.Value,
                        Total = request.Total,
                        CreatedBy = request.CreatedBy,
                        CreatedAt = DateTime.UtcNow,
                        SalesOrderId = response.Data.Id,
                        VariationId = request.VariationId,
                        Barcode = request.Barcode
                    });
                };
                
                return new(response.Data, 201, "Pedido de Venda criado com sucesso.");
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
                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<SalesOrder?>> FinishAsync(FinishSalesOrderDTO request)
        {
            try
            {
                ResponseApi<SalesOrder?> salesOrderResponse = await repository.GetByIdAsync(request.Id);
                if(salesOrderResponse.Data is null) return new(null, 404, "Falha ao finalizar Pedido de Venda");

                ResponseApi<List<SalesOrderItem>> items = await salesOrderItemRepository.GetBySalesOrderIdAsync(request.Id, salesOrderResponse.Data.Plan, salesOrderResponse.Data.Company, salesOrderResponse.Data.Store);
                if(items.Data is not null)
                {
                    foreach (SalesOrderItem salesOrderItem in items.Data)
                    {
                        ResponseApi<Stock?> stock = await stockRepository.GetVerifyStock(salesOrderItem.ProductId, salesOrderItem.Plan, salesOrderItem.Company, salesOrderItem.Store);
                        ResponseApi<Product?> product = await productRepository.GetByIdAsync(salesOrderItem.ProductId);
                        
                        if(product.Data is null) return new(null, 404, "Algum dos Produtos não tem estoque disponível");
                        
                        if(stock.Data is null) return new(null, 404, $"O Produto [{product.Data.Code} - {product.Data.Name}] não tem estoque disponível");

                        if(product.Data.HasSerial == "yes")
                        {
                            bool hasStockAvailable = false;
                            foreach (var variation in stock.Data.Variations)
                            {
                                VariationItemSerial? serial = variation.Serials.Where(s => s.Code == salesOrderItem.Serial && s.HasAvailable).FirstOrDefault();
                                if(serial is not null) 
                                {
                                    serial.HasAvailable = false;
                                    hasStockAvailable = true;
                                };
                            };

                            if(!hasStockAvailable) return new(null, 404, $"O Produto [{product.Data.Code} - {product.Data.Name} | Serial: {salesOrderItem.Serial}] não tem estoque disponível");

                            stock.Data.UpdatedAt = DateTime.UtcNow;
                            stock.Data.UpdatedBy = request.UpdatedBy;
                            stock.Data.Quantity -= 1;

                            await stockRepository.UpdateAsync(stock.Data);
                        }

                    }
                };

                salesOrderResponse.Data.UpdatedAt = DateTime.UtcNow;
                salesOrderResponse.Data.UpdatedBy = request.UpdatedBy;
                salesOrderResponse.Data.Status = "Finalizado";
                salesOrderResponse.Data.Payment = new () 
                {
                    Currier = request.Currier,
                    DiscountType = request.DiscountType,
                    DiscountValue = request.DiscountValue,
                    Freight = request.Freight,
                    NumberOfInstallments = request.NumberOfInstallments,
                    PaymentMethodId = request.PaymentMethodId
                };
                ResponseApi<SalesOrder?> response = await repository.UpdateAsync(salesOrderResponse.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao finalizar Pedido de Venda");
                
                await exchangeService.CreateReleasedStockAsync(new CreateExchangeDTO() { Plan = request.Plan, Company = request.Company, Store = request.Store, UpdatedBy = request.UpdatedBy });
                
                return new(response.Data, 200, "Pedido de Venda Finalizado com sucesso");
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