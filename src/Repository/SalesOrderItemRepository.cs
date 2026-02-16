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
    public class SalesOrderItemRepository(AppDbContext context) : ISalesOrderItemRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<SalesOrderItem> pagination)
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
                MongoUtil.Lookup("attachments", ["$productId"], ["$parentId"], "_images", [["deleted", false]], 1),
                MongoUtil.Lookup("stock", ["$productId"], ["$productId"], "_stock", [["deleted", false]], 1),
                MongoUtil.Lookup("exchanges", ["$_id"], ["$salesOrderItemId"], "_exchange", [["deleted", false]]),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"productName", MongoUtil.First("_product.name")},
                    {"productHasSerial", MongoUtil.First("_product.hasSerial")},
                    {"productHasVariations", MongoUtil.First("_product.hasVariations")},
                    {"productVariations", MongoUtil.First("_product.variations")},
                    {"averageCost", MongoUtil.First("_product.averageCost")},
                    {"image", MongoUtil.First("_images.uri")},
                    {"stockVariations", MongoUtil.First("_stock")},
                    {"exchanges", "$_exchange"}
                }),
                
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_product", 0},
                    {"_images", 0},
                    {"_stock", 0},
                    {"_exchange", 0}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.SalesOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
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

            BsonDocument? response = await context.SalesOrderItems.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Lojas não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    
    public async Task<ResponseApi<SalesOrderItem?>> GetByIdAsync(string id)
    {
        try
        {
            SalesOrderItem? salesOrderItem = await context.SalesOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(salesOrderItem);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<ResponseApi<SalesOrderItem?>> GetByProductIdAsync(string productId, string barcode, string plan, string company, string store)
    {
        try
        {
            SalesOrderItem? salesOrderItem = await context.SalesOrderItems.Find(x => x.ProductId == productId && x.Barcode == barcode && x.Plan == plan && x.Company == company && x.Store == store && !x.Deleted).FirstOrDefaultAsync();
            return new(salesOrderItem);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<ResponseApi<List<SalesOrderItem>>> GetBySalesOrderIdAsync(string salesOrderId, string plan, string company, string store)
    {
        try
        {
            List<SalesOrderItem> salesOrderItems = await context.SalesOrderItems.Find(x => x.SalesOrderId == salesOrderId && x.Plan == plan && x.Company == company && x.Store == store && !x.Deleted).ToListAsync();
            return new(salesOrderItems);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<SalesOrderItem> pagination)
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

        List<BsonDocument> results = await context.SalesOrderItems.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SalesOrderItem?>> CreateAsync(SalesOrderItem salesOrderItem)
    {
        try
        {
            await context.SalesOrderItems.InsertOneAsync(salesOrderItem);

            return new(salesOrderItem, 201, "Lojas criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Lojas");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<SalesOrderItem?>> UpdateAsync(SalesOrderItem salesOrderItem)
    {
        try
        {
            await context.SalesOrderItems.ReplaceOneAsync(x => x.Id == salesOrderItem.Id, salesOrderItem);

            return new(salesOrderItem, 201, "Lojas atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Lojas");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<SalesOrderItem>> DeleteAsync(string id)
    {
        try
        {
            SalesOrderItem? salesOrderItem = await context.SalesOrderItems.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(salesOrderItem is null) return new(null, 404, "Lojas não encontrado");
            salesOrderItem.Deleted = true;
            salesOrderItem.DeletedAt = DateTime.UtcNow;

            await context.SalesOrderItems.ReplaceOneAsync(x => x.Id == id, salesOrderItem);

            return new(salesOrderItem, 204, "Lojas excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Lojas");
        }
    }
    #endregion
}
}