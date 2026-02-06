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
    public class SalesOrderRepository(AppDbContext context) : ISalesOrderRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<SalesOrder> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                MongoUtil.Lookup("employees", ["$sellerId"], ["$_id"], "_seller", [["deleted", false]], 1),
                MongoUtil.Lookup("users", ["$sellerId"], ["$_id"], "_user", [["deleted", false]], 1),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"customerName", MongoUtil.First("_customer.tradeName")},
                    {"userName", MongoUtil.First("_user.name")},
                    {"employeeName", MongoUtil.First("_seller.name")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_customer", 0}, 
                    {"_user", 0}, 
                    {"_seller", 0}, 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.SalesOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
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
                
                MongoUtil.Lookup("customers", ["$customerId"], ["$_id"], "_customer", [["deleted", false]], 1),
                MongoUtil.Lookup("employees", ["$sellerId"], ["$_id"], "_seller", [["deleted", false]], 1),
                MongoUtil.Lookup("users", ["$sellerId"], ["$_id"], "_user", [["deleted", false]], 1),


                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"customerName", MongoUtil.First("_customer.tradeName")},
                    {"userName", MongoUtil.First("_user.name")},
                    {"employeeName", MongoUtil.First("_seller.name")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"_customer", 0},
                    {"_user", 0},
                    {"_seller", 0},
                }),
            ];

            BsonDocument? response = await context.SalesOrders.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Lojas não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    
    public async Task<ResponseApi<SalesOrder?>> GetByIdAsync(string id)
    {
        try
        {
            SalesOrder? address = await context.SalesOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    
    public async Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId)
    {
        try
        {
            long code = await context.SalesOrders.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Próximo Código");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<SalesOrder> pagination)
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

        List<BsonDocument> results = await context.SalesOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<SalesOrder?>> CreateAsync(SalesOrder address)
    {
        try
        {
            await context.SalesOrders.InsertOneAsync(address);

            return new(address, 201, "Lojas criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Lojas");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<SalesOrder?>> UpdateAsync(SalesOrder address)
    {
        try
        {
            await context.SalesOrders.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Lojas atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Lojas");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<SalesOrder>> DeleteAsync(string id)
    {
        try
        {
            SalesOrder? address = await context.SalesOrders.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Lojas não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.SalesOrders.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Lojas excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Lojas");
        }
    }
    #endregion
}
}