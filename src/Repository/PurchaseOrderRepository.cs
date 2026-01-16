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
    public class PurchaseOrderRepository(AppDbContext context) : IPurchaseOrderRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PurchaseOrder> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                MongoUtil.Lookup("purchase_order_items", ["$id"], ["$purchaseOrderId"], "_purchaseOrderItem", [["deleted", false]], 1),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", 1},
                    {"code", 1},
                    {"status", 1},
                    {"date", 1},
                    {"total", 1},
                    {"discount", 1},
                    {"createdAt", 1},
                    {"items", "$_purchaseOrderItem"}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.PurchaseOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Pedido de Compras");
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

            BsonDocument? response = await context.PurchaseOrders.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Pedido de Compras não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Pedido de Compras");
        }
    }
    
    public async Task<ResponseApi<PurchaseOrder?>> GetByIdAsync(string id)
    {
        try
        {
            PurchaseOrder? address = await context.PurchaseOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Pedido de Compras");
        }
    }
    
    public async Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId)
    {
        try
        {
            long code = await context.PurchaseOrders.Find(x => x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Pedido de Compras");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<PurchaseOrder> pagination)
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

        List<BsonDocument> results = await context.PurchaseOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<PurchaseOrder?>> CreateAsync(PurchaseOrder address)
    {
        try
        {
            await context.PurchaseOrders.InsertOneAsync(address);

            return new(address, 201, "Pedido de Compras criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Pedido de Compras");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<PurchaseOrder?>> UpdateAsync(PurchaseOrder address)
    {
        try
        {
            await context.PurchaseOrders.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Pedido de Compras atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Pedido de Compras");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<PurchaseOrder>> DeleteAsync(string id)
    {
        try
        {
            PurchaseOrder? address = await context.PurchaseOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Pedido de Compras não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.PurchaseOrders.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Pedido de Compras excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Pedido de Compras");
        }
    }
    #endregion
}
}