using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class PurchaseOrderService(IPurchaseOrderRepository repository, IPurchaseOrderItemRepository purchaseOrderItemRepository, IStockRepository stockRepository, IMapper _mapper) : IPurchaseOrderService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<PurchaseOrder> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> PurchaseOrders = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(PurchaseOrders.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> PurchaseOrder = await repository.GetByIdAggregateAsync(id);
            if(PurchaseOrder.Data is null) return new(null, 404, "Pedido de Compra não encontrado");
            return new(PurchaseOrder.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<PurchaseOrder?>> CreateAsync(CreatePurchaseOrderDTO request)
    {
        try
        {
            PurchaseOrder product = _mapper.Map<PurchaseOrder>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Company, request.Store);
            product.Code = code.Data.ToString().PadLeft(6, '0');
            product.Status = "Rascunho";
            ResponseApi<PurchaseOrder?> response = await repository.CreateAsync(product);

            if(response.Data is null) return new(null, 400, "Falha ao criar Pedido de Compra.");
            return new(response.Data, 201, "Pedido de Compra criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<PurchaseOrder?>> UpdateAsync(UpdatePurchaseOrderDTO request)
    {
        try
        {
            ResponseApi<PurchaseOrder?> PurchaseOrderResponse = await repository.GetByIdAsync(request.Id);
            if(PurchaseOrderResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            PurchaseOrder PurchaseOrder = _mapper.Map<PurchaseOrder>(request);
            PurchaseOrder.UpdatedAt = DateTime.UtcNow;
            PurchaseOrder.Code = PurchaseOrderResponse.Data.Code;
            PurchaseOrder.Status = PurchaseOrderResponse.Data.Status;

            ResponseApi<PurchaseOrder?> response = await repository.UpdateAsync(PurchaseOrder);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<PurchaseOrder?>> UpdateApprovalAsync(UpdatePurchaseOrderDTO request)
    {
        try
        {
            ResponseApi<PurchaseOrder?> PurchaseOrderResponse = await repository.GetByIdAsync(request.Id);
            if(PurchaseOrderResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            PurchaseOrderResponse.Data.UpdatedAt = DateTime.UtcNow;
            PurchaseOrderResponse.Data.UpdatedBy = request.UpdatedBy;
            PurchaseOrderResponse.Data.Status = "Finalizado";

            ResponseApi<List<PurchaseOrderItem>> items = await purchaseOrderItemRepository.GetByPurchaseOrderIdAsync(request.Id);
            if(items.Data is not null)
            {
                foreach(PurchaseOrderItem item in items.Data)
                {
                    if(item.MoveStock == "yes")
                    {
                        for(int i = 0; i < item.Quantity; i++)
                        {
                            var grupos = item.Variations
                                .GroupBy(v => v.Key)
                                .Select(g => g.Select(v => v.Value).ToList())
                                .ToList();

                            var combinacoes = grupos.Aggregate(
                                (IEnumerable<IEnumerable<string>>)new[] { Enumerable.Empty<string>() },
                                (acc, grupo) => from a in acc from g in grupo select a.Append(g)
                            );

                            foreach (var variations in combinacoes)
                            {
                                List<Variation> myVariations = [];
                                foreach (string variation in variations)
                                {
                                    var itemKey = item.Variations.Where(v => v.Value == variation).FirstOrDefault();
                                    if(itemKey is not null)
                                    {
                                        myVariations.Add(new () {Key = itemKey.Key, Value = variation});
                                    }
                                };

                                string cost = item.Cost.ToString().PadLeft(7, '0');
                                string quantity = item.Quantity.ToString().PadLeft(4, '0');

                                await stockRepository.CreateAsync(new ()
                                {
                                    Code = $"{cost}{quantity}",
                                    Active = true,
                                    Cost = item.Cost,
                                    CostDiscount = item.CostDiscount,
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedBy = request.CreatedBy,
                                    Deleted = false,
                                    DeletedAt = null,
                                    Price = item.Price,
                                    PriceDiscount = item.PriceDiscount,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    SupplierId = item.SupplierId,
                                    Variations = myVariations
                                });
                            }
                        }
                    }
                };
            };

            ResponseApi<PurchaseOrder?> response = await repository.UpdateAsync(PurchaseOrderResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            return new(response.Data, 201, "Finalizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<PurchaseOrder>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<PurchaseOrder> PurchaseOrder = await repository.DeleteAsync(id);
            if(!PurchaseOrder.IsSuccess) return new(null, 400, PurchaseOrder.Message);
            return new(null, 204, "Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 

    #region FUCTIONS
    public (string Key, int Quantidade) ObterKeyMaisRepetida(List<Variation> variacoes)
    {
        if (variacoes == null || !variacoes.Any()) return (string.Empty, 0);

        var resultado = variacoes
            .GroupBy(v => v.Key) 
            .Select(g => new 
            { 
                g.Key, 
                Contagem = g.Count() 
            }) 
            .OrderByDescending(x => x.Contagem) 
            .FirstOrDefault(); 

        return (resultado!.Key, resultado.Contagem);
    }
    #endregion
}
}