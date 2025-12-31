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
    public class ContactRepository(AppDbContext context) : IContactRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Contact> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "generic_tables" }, 
                    { "let", new BsonDocument("department", "$department") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$department" }),
                                        new BsonDocument("$eq", new BsonArray { "$table", "departamento-contato-representante" })
                                    })
                                }
                            })
                        }
                    },
                    { "as", "_department" } 
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_department" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "generic_tables" }, 
                    { "let", new BsonDocument("position", "$position") },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                            {
                                { "$expr", new BsonDocument("$and", new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$code", "$$position" }),
                                        new BsonDocument("$eq", new BsonArray { "$table", "funcao-contato-representante" })
                                    })
                                }
                            })
                        }
                    },
                    { "as", "_position" } 
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$_position" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"departmentDescription", "$_department.description"},
                    {"positionDescription", "$_position.description"}
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"_department", 0}, 
                    {"_position", 0} 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Contacts.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Items");
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
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"name", 1},
                    {"description", 1}
                }),
            ];

            BsonDocument? response = await context.Contacts.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Item não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<ResponseApi<Contact?>> GetByIdAsync(string id)
    {
        try
        {
            Contact? billing = await context.Contacts.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(billing);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Item");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Contact> pagination)
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

        List<BsonDocument> results = await context.Contacts.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Contact?>> CreateAsync(Contact billing)
    {
        try
        {
            await context.Contacts.InsertOneAsync(billing);

            return new(billing, 201, "Item criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Item");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Contact?>> UpdateAsync(Contact billing)
    {
        try
        {
            await context.Contacts.ReplaceOneAsync(x => x.Id == billing.Id, billing);

            return new(billing, 201, "Item atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Item");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Contact>> DeleteAsync(string id)
    {
        try
        {
            Contact? billing = await context.Contacts.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(billing is null) return new(null, 404, "Item não encontrado");
            billing.Deleted = true;
            billing.DeletedAt = DateTime.UtcNow;

            await context.Contacts.ReplaceOneAsync(x => x.Id == id, billing);

            return new(billing, 204, "Item excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Item");
        }
    }
    #endregion
}
}