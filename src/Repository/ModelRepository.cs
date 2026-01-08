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
    public class ModelRepository(AppDbContext context) : IModelRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Model> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
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

            List<BsonDocument> results = await context.Models.Aggregate<BsonDocument>(pipeline).ToListAsync();
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

            BsonDocument? response = await context.Models.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Lojas não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    
    public async Task<ResponseApi<Model?>> GetByIdAsync(string id)
    {
        try
        {
            Model? address = await context.Models.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Lojas");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Model> pagination)
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

        List<BsonDocument> results = await context.Models.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Model?>> CreateAsync(Model address)
    {
        try
        {
            await context.Models.InsertOneAsync(address);

            return new(address, 201, "Lojas criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Lojas");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Model?>> UpdateAsync(Model address)
    {
        try
        {
            await context.Models.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Lojas atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Lojas");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Model>> DeleteAsync(string id)
    {
        try
        {
            Model? address = await context.Models.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Lojas não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.Models.ReplaceOneAsync(x => x.Id == id, address);

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