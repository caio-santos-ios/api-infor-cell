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
    public class BoxRepository(AppDbContext context) : IBoxRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Box> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                MongoUtil.Lookup("users", ["$opendBy"], ["$_id"], "_user", [["deleted", false]], 1),
                MongoUtil.Lookup("employees", ["$opendBy"], ["$_id"], "_employee", [["deleted", false]], 1),

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
                    {"value", 1},
                    {"status", 1},
                    {"createdAt", 1},
                    {"twoSteps", 1},
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

            List<BsonDocument> results = await context.Boxes.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Caixa");
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

            BsonDocument? response = await context.Boxes.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Caixa não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Caixa");
        }
    }

    public async Task<ResponseApi<dynamic?>> GetByCreatedIdAggregateAsync(string createdBy)
    {
        try
        {
            BsonDocument[] pipeline = [
                new("$match", new BsonDocument{
                    {"createdBy", createdBy},
                    {"deleted", false},
                    {"status", "opened"}
                }),
                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.Boxes.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Caixa não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Caixa");
        }
    }
    
    public async Task<ResponseApi<Box?>> GetByIdAsync(string id)
    {
        try
        {
            Box? box = await context.Boxes.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(box);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Caixa");
        }
    }
    public async Task<ResponseApi<Box?>> GetByCreatedIdAsync(string createdBy)
    {
        try
        {
            Box? box = await context.Boxes.Find(x => x.CreatedBy == createdBy && !x.Deleted && x.Status == "opened").FirstOrDefaultAsync();
            return new(box);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Caixa");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Box> pagination)
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

        List<BsonDocument> results = await context.Boxes.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Box?>> CreateAsync(Box box)
    {
        try
        {
            await context.Boxes.InsertOneAsync(box);

            return new(box, 201, "Caixa criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Caixa");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Box?>> UpdateAsync(Box box)
    {
        try
        {
            await context.Boxes.ReplaceOneAsync(x => x.Id == box.Id, box);

            return new(box, 201, "Caixa atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Caixa");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Box>> DeleteAsync(string id)
    {
        try
        {
            Box? box = await context.Boxes.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(box is null) return new(null, 404, "Caixa não encontrado");
            box.Deleted = true;
            box.DeletedAt = DateTime.UtcNow;

            await context.Boxes.ReplaceOneAsync(x => x.Id == id, box);

            return new(box, 204, "Caixa excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Caixa");
        }
    }
    #endregion
}
}