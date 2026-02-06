using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;
using MongoDB.Driver;

namespace api_infor_cell.src.Services
{
    public class TransferService(ITransferRepository repository, IStockRepository stockRepository, IStoreRepository storeRepository, IStockService stockService, IMapper _mapper) : ITransferService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Transfer> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Transfers = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Transfers.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Transfer = await repository.GetByIdAggregateAsync(id);
            if(Transfer.Data is null) return new(null, 404, "Tranferência não encontrada");
            return new(Transfer.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Transfer?>> CreateAsync(CreateTransferDTO request)
    {
        try
        {
            ResponseApi<List<Stock>> stocksOriginResponse = await stockRepository.GetStockTransfer(
            request.ProductId, 
            request.Barcode, 
            request.ProductHasSerial,
            request.Serial,
            request.Plan, 
            request.Company, 
            request.StoreOriginId);

            if (stocksOriginResponse.Data == null || stocksOriginResponse.Data.Count == 0) return new(null, 404, "Estoque de origem não encontrado para esta variação.");
            ResponseApi<Store?> store = await storeRepository.GetByIdAsync(request.Store);

            if(request.ProductHasSerial == "yes")
            {
                if(string.IsNullOrEmpty(request.Barcode)) return new(null, 400, "A Variação é obrigatória.");
                if(string.IsNullOrEmpty(request.Serial)) return new(null, 400, "O Serial é obrigatório.");
                System.Console.WriteLine(stocksOriginResponse.Data.Count);
                Stock? stock = stocksOriginResponse.Data.Where(x => x.Variations.Where(v => v.Barcode == request.Barcode && v.VariationId == request.VariationId && v.Serials.Where(s => s.Code == request.Serial).Count() > 0).Count() > 0).FirstOrDefault();
                if (stock is null) return new(null, 404, "Estoque de origem não encontrado para esta variação.");

                VariationProduct? variationProduct = stock.Variations.Where(v => v.Barcode == request.Barcode && v.VariationId == request.VariationId).FirstOrDefault();
                if (variationProduct is null) return new(null, 404, "Estoque de origem não encontrado para esta variação.");
                Util.ConsoleLog(variationProduct);
                VariationItemSerial? variationItemSerial = variationProduct.Serials.Where(s => s.Code == request.Serial && s.HasAvailable).FirstOrDefault();
                if (variationItemSerial is null) return new(null, 404, "Estoque de origem não encontrado para este serial.");

                string costPart = stock.Cost.ToString().PadLeft(7, '0');
                string quantityPart = "1".PadLeft(4, '0');
                CreateStockDTO newStock = new()
                {
                    ProductId = request.ProductId,
                    SupplierId = stock.SupplierId,
                    Price = stock.Price,
                    Cost = stock.Cost,
                    CostDiscount = stock.CostDiscount,
                    PriceDiscount = stock.PriceDiscount,
                    PurchaseOrderItemId = stock.PurchaseOrderItemId,
                    Store = request.StoreDestinationId,
                    Quantity = 1,
                    Barcode = request.Barcode,
                    Variations = new List<VariationProduct>()
                    {
                        new VariationProduct()
                        {
                            Attributes = variationProduct.Attributes,
                            Barcode = variationProduct.Barcode,
                            Stock = variationProduct.Stock,
                            Value = variationProduct.Value,
                            VariationId = variationProduct.VariationId,
                            VariationItemId = variationProduct.VariationItemId,
                            Serials = new List<VariationItemSerial>()
                            {
                                new()
                                {
                                    Code = variationItemSerial.Code,
                                    HasAvailable = variationItemSerial.HasAvailable,
                                    Cost = variationItemSerial.Cost,
                                    Price = variationItemSerial.Price
                                }
                            }
                        }
                    },
                    VariationsCode = stock.VariationsCode,
                    Company = request.Company,
                    Plan = request.Plan,
                    Origin = store.Data is null ? "Transferência recebida por loja" :  $"Transferência recebida da loja {store.Data.CorporateName}",
                    CreatedBy = request.CreatedBy
                };

                await stockService.CreateAsync(newStock);

                List<VariationProduct> variationsOrigin = stock.Variations.Where(v => v.Barcode == request.Barcode && v.VariationId == request.VariationId).ToList();
                foreach (VariationProduct variation in variationsOrigin)
                {
                    List<VariationItemSerial> serialOrigin = variation.Serials.Where(s => s.Code != request.Serial).ToList();

                    variation.Serials = serialOrigin;
                    variation.Stock -= 1;
                }

                stock.Quantity -= 1;
                stock.Variations = variationsOrigin;

                await stockRepository.UpdateAsync(stock);

                await repository.CreateAsync(new Transfer()
                {
                    Plan = request.Plan,
                    Company = request.Company,
                    Store = request.Store,
                    CreatedBy = request.CreatedBy,
                    PurchaseOrderItemId = stock.PurchaseOrderItemId,
                    StockId = stock.Id,
                    StoreDestinationId = request.StoreDestinationId,
                    StoreOriginId = request.StoreOriginId,
                    Quantity = 1
                });
            }
            else
            {
                // decimal totalBalanceOrigin = stocksOriginResponse.Data.Sum(x => x.Quantity);

                // if (totalBalanceOrigin < request.Quantity) return new(null, 400, $"Saldo insuficiente na origem. Disponível: {totalBalanceOrigin:N2}");

                // var stockOrigin = stocksOriginResponse.Data.First();

                // stockOrigin.Quantity -= request.Quantity;
                // stockOrigin.UpdatedAt = DateTime.UtcNow;
                // stockOrigin.UpdatedBy = request.UpdatedBy;
                
                // await stockRepository.UpdateAsync(stockOrigin);

                // ResponseApi<List<Stock>> stocksDestinationResponse = await stockRepository.GetStockTransfer(
                //     request.ProductId, 
                //     request.Barcode, 
                //     request.Plan, 
                //     request.Company, 
                //     request.StoreDestinationId);

                // Stock? stockDest = stocksDestinationResponse.Data?.FirstOrDefault();

                // if (stockDest != null)
                // {
                //     stockDest.Quantity += request.Quantity;
                //     stockDest.UpdatedAt = DateTime.UtcNow;
                //     stockDest.UpdatedBy = request.UpdatedBy;
                //     stockDest.Origin = store.Data is null ? "Transferência recebida por loja" :  $"Transferência recebida da loja {store.Data.CorporateName}";
                    
                //     await stockRepository.UpdateAsync(stockDest);
                // }
                // else
                // {                
                //     Stock newStock = new()
                //     {
                //         ProductId = request.ProductId,
                //         Store = request.StoreDestinationId,
                //         Quantity = request.Quantity,
                //         Barcode = request.Barcode,
                //         // Variation = stockOrigin.Variation, 
                //         VariationsCode = stockOrigin.VariationsCode,
                //         Company = request.Company,
                //         Plan = request.Plan,
                //         Active = true,
                //         Deleted = false,
                //         Origin = store.Data is null ? "Transferência recebida por loja" :  $"Transferência recebida da loja {store.Data.CorporateName}",
                //         CreatedAt = DateTime.UtcNow,
                //         CreatedBy = request.CreatedBy
                //     };

                //     await stockRepository.CreateAsync(newStock);
                // }
            }

            return new(null, 201, "Transferência concluída e registrada com sucesso!");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Transfer?>> UpdateAsync(UpdateTransferDTO request)
    {
        try
        {
            ResponseApi<Transfer?> TransferResponse = await repository.GetByIdAsync(request.Id);
            if(TransferResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Transfer Transfer = _mapper.Map<Transfer>(request);
            Transfer.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Transfer?> response = await repository.UpdateAsync(Transfer);
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
    public async Task<ResponseApi<Transfer>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Transfer> Transfer = await repository.DeleteAsync(id);
            if(!Transfer.IsSuccess) return new(null, 400, Transfer.Message);
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