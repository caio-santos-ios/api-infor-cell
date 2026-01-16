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
    public class ProductRepository(AppDbContext context) : IProductRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Product> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                MongoUtil.Lookup("models", ["$modelId"], ["$_id"], "_model", [["deleted", false]], 1),
                MongoUtil.Lookup("categories", ["$categoryId"], ["$_id"], "_category", [["deleted", false]], 1),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"code", 1},
                    {"name", 1},
                    {"description", 1},
                    {"createdAt", 1},
                    {"modelName", MongoUtil.First("_model.name")},
                    {"categoryName", MongoUtil.First("_category.name")},
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }    
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Product> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"code", 1},
                    {"name", 1},
                    {"variations", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

            BsonDocument? response = await context.Products.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Lojas não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<ResponseApi<Product?>> GetByIdAsync(string id)
    {
        try
        {
            Product? address = await context.Products.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId)
    {
        try
        {
            long code = await context.Products.Find(x => x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Lojas");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Product> pagination)
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

        List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Product?>> CreateAsync(Product address)
    {
        try
        {
            await context.Products.InsertOneAsync(address);

            return new(address, 201, "Lojas criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Lojas");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Product?>> UpdateAsync(Product address)
    {
        try
        {
            await context.Products.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Lojas atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Lojas");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Product>> DeleteAsync(string id)
    {
        try
        {
            Product? address = await context.Products.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Lojas não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.Products.ReplaceOneAsync(x => x.Id == id, address);

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