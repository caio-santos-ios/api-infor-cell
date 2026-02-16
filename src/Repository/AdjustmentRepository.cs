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
    public class AdjustmentRepository(AppDbContext context) : IAdjustmentRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Adjustment> pagination)
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
                MongoUtil.Lookup("users", ["$createdBy"], ["$_id"], "_user", [["deleted", false]], 1),
                MongoUtil.Lookup("employees", ["$createdBy"], ["$_id"], "_employee", [["deleted", false]], 1),

                new("$addFields", new BsonDocument {
                    {"userName", MongoUtil.First("_user.name")},
                    {"employeeName", MongoUtil.First("_employee.name")},
                }),
                new("$addFields", new BsonDocument {
                    {"existed", MongoUtil.ValidateNull("userName", "")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"type", 1},
                    {"createdAt", 1},
                    {"quantity", 1},
                    {"productName", MongoUtil.First("_product.name")},
                    {"user", new BsonDocument("$cond", new BsonDocument 
                        {
                            { "if", new BsonDocument("$eq", new BsonArray { 
                                "$existed", 
                                "" 
                            }) },
                            { "then", "$employeeName" },
                            { "else", "$userName" }
                        })
                    }
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Adjustments.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ajuste");
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
                
                MongoUtil.Lookup("products", ["$productId"], ["$_id"], "_product", [["deleted", false]], 1),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"productName", MongoUtil.First("_product.name")},
                    {"hasProductSerial", MongoUtil.First("_product.hasSerial")},
                    {"hasProductVariations", MongoUtil.First("_product.hasVariations")}
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.Adjustments.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Ajuste não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ajuste");
        }
    }
    
    public async Task<ResponseApi<Adjustment?>> GetByIdAsync(string id)
    {
        try
        {
            Adjustment? adjustment = await context.Adjustments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(adjustment);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Ajuste");
        }
    }
    public async Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId)
    {
        try
        {
            long code = await context.Adjustments.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Ajuste");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Adjustment> pagination)
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

        List<BsonDocument> results = await context.Adjustments.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Adjustment?>> CreateAsync(Adjustment adjustment)
    {
        try
        {
            await context.Adjustments.InsertOneAsync(adjustment);

            return new(adjustment, 201, "Ajuste criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Ajuste");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Adjustment?>> UpdateAsync(Adjustment adjustment)
    {
        try
        {
            await context.Adjustments.ReplaceOneAsync(x => x.Id == adjustment.Id, adjustment);

            return new(adjustment, 201, "Ajuste atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Ajuste");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Adjustment>> DeleteAsync(string id)
    {
        try
        {
            Adjustment? adjustment = await context.Adjustments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(adjustment is null) return new(null, 404, "Ajuste não encontrado");
            adjustment.Deleted = true;
            adjustment.DeletedAt = DateTime.UtcNow;

            await context.Adjustments.ReplaceOneAsync(x => x.Id == id, adjustment);

            return new(adjustment, 204, "Ajuste excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Ajuste");
        }
    }
    #endregion
}
}