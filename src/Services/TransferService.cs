using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class TransferService(ITransferRepository repository, IStockRepository stockRepository, IMapper _mapper) : ITransferService
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
            var response = await stockRepository.GetByPurchaseItemIdAsync(
                request.PurchaseOrderItemId, 
                request.Plan, 
                request.Company, 
                request.StoreOriginId);

            if (response.Data == null || !response.Data.Any())
                return new(null, 404, "Estoque não encontrado na loja de origem.");

            decimal quantityRemaining = request.Quantity;
            decimal totalAvailable = response.Data.Sum(s => Convert.ToDecimal(s.Quantity));

            if (totalAvailable < quantityRemaining)
                return new(null, 400, "Saldo insuficiente para realizar a transferência.");

            foreach (var stock in response.Data)
            {
                if (quantityRemaining <= 0) break;

                decimal currentStockQty = Convert.ToDecimal(stock.Quantity);
                if (currentStockQty <= 0) continue;

                decimal amountToMove = Math.Min(currentStockQty, quantityRemaining);

                var history = new Transfer
                {
                    Store = request.Store,
                    StoreOriginId = request.StoreOriginId,
                    StoreDestinationId = request.StoreDestinationId,
                    PurchaseOrderItemId = request.PurchaseOrderItemId,
                    StockId = stock.Id, 
                    Quantity = amountToMove,
                    Company = request.Company,
                    Plan = request.Plan,
                    CreatedAt = DateTime.UtcNow
                };

                if (currentStockQty == amountToMove)
                {
                    stock.Store = request.StoreDestinationId;
                    await stockRepository.UpdateAsync(stock);
                }
                else
                {
                    var newStock = stock;
                    newStock.Store = request.StoreDestinationId;
                    newStock.Quantity = amountToMove;
                    
                    var nextCode = await stockRepository.GetNextCodeAsync(newStock.Plan, newStock.Company, newStock.Store);
                    newStock.Code = nextCode.Data.ToString().PadLeft(6, '0');

                    await stockRepository.CreateAsync(newStock);

                    stock.Quantity = currentStockQty - amountToMove;
                    await stockRepository.UpdateAsync(stock);
                }

                await repository.CreateAsync(history);

                quantityRemaining -= amountToMove;
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