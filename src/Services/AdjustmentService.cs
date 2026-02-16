using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class AdjustmentService(IAdjustmentRepository repository, IStockService stockService, IStockRepository stockRepository, IMapper _mapper) : IAdjustmentService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Adjustment> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Adjustmentts = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Adjustmentts.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Adjustmentt = await repository.GetByIdAggregateAsync(id);
            if(Adjustmentt.Data is null) return new(null, 404, "Ajuste não encontrada");
            return new(Adjustmentt.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Adjustment?>> CreateAsync(CreateAdjustmentDTO request)
    {
        try
        {
            Adjustment adjustment = _mapper.Map<Adjustment>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Plan, request.Company, request.Store);
            adjustment.Code = code.Data.ToString().PadLeft(6, '0');
            ResponseApi<Adjustment?> response = await repository.CreateAsync(adjustment);

            if(response.Data is null) return new(null, 400, "Falha ao criar Ajuste.");
            
            if(request.Type == "Entrada")
            {
                await stockService.CreateAsync(new ()
                {
                    Plan = request.Plan,
                    Company = request.Company,
                    Store = request.Store,
                    ProductId = request.ProductId,
                    Cost = request.Cost,
                    CostDiscount = request.CostDiscount,
                    Price = request.Price,
                    PriceDiscount = request.PriceDiscount,
                    Barcode = request.Barcode,
                    Quantity = request.Quantity,
                    Variations = request.Variations,
                    VariationsCode = request.VariationsCode,
                    ForSale = "yes",
                    CreatedBy = request.CreatedBy,
                    Origin = "adjustment",
                    OriginId = response.Data.Id,
                    OriginDescription = $"Ajuste - Nº {response.Data.Code}",
                    PurchaseOrderItemId = "",
                    SupplierId = ""
                });
            }
            else
            {
                ResponseApi<List<Stock>> stocks = await stockRepository.GetByProductId(request.ProductId, request.Plan, request.Company, request.Store);
                if(stocks.Data is not null)
                {
                    List<Stock> listStock = stocks.Data.Where(x => x.Quantity > 0 && x.HasProductSerial == request.HasSerial && x.HasProductVariations == request.HasVariations).ToList();

                    if(request.HasVariations == "yes") 
                    {
                        if(request.HasSerial == "yes")
                        {
                            System.Console.WriteLine("teste");
                        }
                        else
                        {
                            foreach (Guid itemCode in request.Codes)
                            {
                                var stock = listStock.Where(x => x.Variations.Where(v => v.Code == itemCode).Count() > 0).FirstOrDefault();
                                if(stock is not null)
                                {
                                    stock.Variations = stock.Variations.Where(v => v.Code != itemCode).ToList();
                                    stock.UpdatedAt = DateTime.Now;
                                    stock.UpdatedBy = request.UpdatedBy;
                                    stock.Quantity -= 1;

                                    await stockRepository.UpdateAsync(stock);
                                }                                
                            }
                        }
                    }
                    else
                    {
                        foreach (Stock stock in stocks.Data)
                        {
                            if(stock.Quantity == 0) continue;

                            decimal accumulated = 0;
                            if(accumulated < request.Quantity) 
                            {
                                decimal total = stock.Quantity - request.Quantity;
                                accumulated += total < 0 ? request.Quantity : total;

                                stock.UpdatedAt = DateTime.Now;
                                stock.UpdatedBy = request.UpdatedBy;
                                stock.Quantity = total < 0 ? 0 : total;

                                await stockRepository.UpdateAsync(stock);
                            }
                        }
                    };
                }
            };

            return new(response.Data, 201, "Ajuste criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Adjustment?>> UpdateAsync(UpdateAdjustmentDTO request)
    {
        try
        {
            ResponseApi<Adjustment?> adjustmentResponse = await repository.GetByIdAsync(request.Id);
            if(adjustmentResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            Adjustment adjustment = _mapper.Map<Adjustment>(request);
            adjustment.UpdatedAt = DateTime.UtcNow;
            adjustment.CreatedAt = adjustmentResponse.Data.CreatedAt;

            ResponseApi<Adjustment?> response = await repository.UpdateAsync(adjustment);
            if(!response.IsSuccess || response.Data is null) return new(null, 400, "Falha ao atualizar");

            if(request.Type == "Entrada")
            {
                ResponseApi<Stock?> stock = await stockRepository.GetByOriginIdAsync(request.Id);
                if(stock.Data is not null)
                {
                    stock.Data.UpdatedAt = DateTime.Now;
                    stock.Data.UpdatedBy = request.UpdatedBy;
                    stock.Data.Quantity += request.Quantity;
                    stock.Data.Cost = request.Cost;
                    stock.Data.Price = request.Price;
                    stock.Data.PriceDiscount = request.PriceDiscount;
                    stock.Data.Origin = "adjustment";
                    stock.Data.OriginId = response.Data.Id;
                    stock.Data.OriginDescription = $"Ajuste - Nº {response.Data.Code}";

                    await stockRepository.UpdateAsync(stock.Data);
                };
            }
            else
            {
                ResponseApi<List<Stock>> stocks = await stockRepository.GetByProductId(request.ProductId, request.Plan, request.Company, request.Store);

                if(stocks.Data is not null)
                {
                    List<Stock> listStock = stocks.Data.Where(x => x.HasProductSerial == request.HasSerial && x.HasProductVariations == request.HasVariations).ToList();
                    if(request.HasVariations == "yes") 
                    {
                        if(request.HasSerial == "yes")
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        decimal accumulated = 0;
                        foreach (Stock stock in stocks.Data)
                        {
                            if(accumulated < request.Quantity) 
                            {
                                decimal total = stock.Quantity - request.Quantity;
                                accumulated += total < 0 ? request.Quantity : total;

                                stock.UpdatedAt = DateTime.Now;
                                stock.UpdatedBy = request.UpdatedBy;
                                stock.Quantity = total < 0 ? 0 : total;

                                await stockRepository.UpdateAsync(stock);
                            }
                        }
                    };
                }
            }

            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }

    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Adjustment>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Adjustment> Adjustment = await repository.DeleteAsync(id);
            if(!Adjustment.IsSuccess) return new(null, 400, Adjustment.Message);
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