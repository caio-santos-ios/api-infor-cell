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
    public class ExchangeRepository(AppDbContext context) : IExchangeRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Exchange> pagination)
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
                MongoUtil.Lookup("sales_order_items", ["$salesOrderItemId"], ["$_id"], "_salesOrderItem", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"productName", MongoUtil.First("_product.name")},
                    {"salesOrderId", MongoUtil.First("_salesOrderItem.salesOrderId")},
                }),

                MongoUtil.Lookup("sales_orders", ["$salesOrderId"], ["$_id"], "_salesOrder", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument
                {
                    {"salesOrderCode", MongoUtil.First("_salesOrder.code")},
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Exchanges.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Troca");
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

            BsonDocument? response = await context.Exchanges.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Troca não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Troca");
        }
    }
    
    public async Task<ResponseApi<List<dynamic>>> GetBySalesOrderItemIdAggregateAsync(string salesOrderItemId)
    {
        try
        {
            BsonDocument[] pipeline = [
                new("$match", new BsonDocument{
                    {"salesOrderItemId", salesOrderItemId},
                    {"deleted", false}
                }),
                
                MongoUtil.Lookup("products", ["$productId"], ["$_id"], "_product", [["deleted", false]], 1),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"productCode", MongoUtil.First("_product.code")},
                    {"productName", MongoUtil.First("_product.name")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            List<BsonDocument> results = await context.Exchanges.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Troca");
        }
    }
    
    public async Task<ResponseApi<Exchange?>> GetByIdAsync(string id)
    {
        try
        {
            Exchange? exchange = await context.Exchanges.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(exchange);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Troca");
        }
    }
    public async Task<ResponseApi<List<Exchange>>> GetReleasedStockAsync(string plan, string company, string store)
    {
        try
        {
            List<Exchange> exchanges = await context.Exchanges.Find(x => x.Plan == plan && x.Company == company && x.Store == store && !x.Deleted).ToListAsync();
            return new(exchanges);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Troca");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Exchange> pagination)
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

        List<BsonDocument> results = await context.Exchanges.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Exchange?>> CreateAsync(Exchange exchange)
    {
        try
        {
            await context.Exchanges.InsertOneAsync(exchange);

            return new(exchange, 201, "Troca criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Troca");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Exchange?>> UpdateAsync(Exchange exchange)
    {
        try
        {
            await context.Exchanges.ReplaceOneAsync(x => x.Id == exchange.Id, exchange);

            return new(exchange, 201, "Troca atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Troca");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Exchange>> DeleteAsync(string id)
    {
        try
        {
            Exchange? exchange = await context.Exchanges.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(exchange is null) return new(null, 404, "Troca não encontrado");
            exchange.Deleted = true;
            exchange.DeletedAt = DateTime.UtcNow;

            await context.Exchanges.ReplaceOneAsync(x => x.Id == id, exchange);

            return new(exchange, 204, "Troca excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Troca");
        }
    }
    #endregion
}
}