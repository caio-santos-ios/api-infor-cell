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
    public class CompanyRepository(AppDbContext context) : ICompanyRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Company> pagination)
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

            List<BsonDocument> results = await context.Companies.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Empresas");
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

            BsonDocument? response = await context.Companies.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Empresa não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Empresa");
        }
    }
    
    public async Task<ResponseApi<Company?>> GetByIdAsync(string id)
    {
        try
        {
            Company? address = await context.Companies.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Empresa");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Company> pagination)
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

        List<BsonDocument> results = await context.Companies.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Company?>> CreateAsync(Company address)
    {
        try
        {
            await context.Companies.InsertOneAsync(address);

            return new(address, 201, "Empresa criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Empresa");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Company?>> UpdateAsync(Company address)
    {
        try
        {
            await context.Companies.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Empresa atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Empresa");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Company>> DeleteAsync(string id)
    {
        try
        {
            Company? address = await context.Companies.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Empresa não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.Companies.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Empresa excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Empresa");
        }
    }
    #endregion
}
}