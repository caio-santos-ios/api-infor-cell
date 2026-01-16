using api_infor_cell.src.Configuration;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace api_infor_cell.src.Repository
{
    public class PurchaseOrderItemRepository(AppDbContext context) : IPurchaseOrderItemRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PurchaseOrderItem> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                MongoUtil.Lookup("products", ["$productId"], ["$_id"], "_product", [["deleted", false]], 1),
                MongoUtil.Lookup("suppliers", ["$supplierId"], ["$_id"], "_supplier", [["deleted", false]], 1),
                
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"code", 1},
                    {"name", 1},
                    {"cost", 1},
                    {"quantity", 1},
                    {"price", 1},
                    {"priceDiscount", 1},
                    {"moveStock", 1},
                    {"createdAt", 1},
                    {"productName", MongoUtil.First("_product.name")},
                    {"supplierName", MongoUtil.First("_supplier.tradeName")}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.PurchaseOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item Pedido de Compra");
        }
    }
    
    public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
    {
        try
        {
            BsonDocument[] pipeline = [
                new("$match", new BsonDocument{
                    {"_id", new ObjectId(id)},
                    {"deleted", false}
                }),
                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.PurchaseOrderItems.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Item Pedido de Compra não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item Pedido de Compra");
        }
    }
    
    public async Task<ResponseApi<PurchaseOrderItem?>> GetByIdAsync(string id)
    {
        try
        {
            PurchaseOrderItem? purchaseOrderItem = await context.PurchaseOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(purchaseOrderItem);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item Pedido de Compra");
        }
    }
    
    public async Task<ResponseApi<List<PurchaseOrderItem>>> GetByPurchaseOrderIdAsync(string purchaseOrderId)
    {
        try
        {
            List<PurchaseOrderItem> items = await context.PurchaseOrderItems.Find(x => x.PurchaseOrderId == purchaseOrderId && !x.Deleted).ToListAsync();
            return new(items);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item Pedido de Compra");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<PurchaseOrderItem> pagination)
    {
        List<BsonDocument> pipeline = new()
        {
            new("$match", pagination.PipelineFilter),
            new("$sort", pagination.PipelineSort),
            new("$addFields", new BsonDocument
            {
                {"id", new BsonDocument("$toString", "$_id")},
            }),
            new("$project", new BsonDocument
            {
                {"_id", 0},
            }),
            new("$sort", pagination.PipelineSort),
        };

        List<BsonDocument> results = await context.PurchaseOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<PurchaseOrderItem?>> CreateAsync(PurchaseOrderItem purchaseOrderItem)
    {
        try
        {
            await context.PurchaseOrderItems.InsertOneAsync(purchaseOrderItem);

            return new(purchaseOrderItem, 201, "Item Pedido de Compra criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Item Pedido de Compra");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<PurchaseOrderItem?>> UpdateAsync(PurchaseOrderItem purchaseOrderItem)
    {
        try
        {
            await context.PurchaseOrderItems.ReplaceOneAsync(x => x.Id == purchaseOrderItem.Id, purchaseOrderItem);

            return new(purchaseOrderItem, 201, "Item Pedido de Compra atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Item Pedido de Compra");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<PurchaseOrderItem>> DeleteAsync(string id)
    {
        try
        {
            PurchaseOrderItem? purchaseOrderItem = await context.PurchaseOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(purchaseOrderItem is null) return new(null, 404, "Item Pedido de Compra não encontrado");
            purchaseOrderItem.Deleted = true;
            purchaseOrderItem.DeletedAt = DateTime.UtcNow;

            await context.PurchaseOrderItems.ReplaceOneAsync(x => x.Id == id, purchaseOrderItem);

            return new(purchaseOrderItem, 204, "Item Pedido de Compra excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Item Pedido de Compra");
        }
    }
    #endregion
}
}