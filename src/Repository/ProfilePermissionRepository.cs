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
    public class ProfilePermissionRepository(AppDbContext context) : IProfilePermissionRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ProfilePermission> pagination)
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

            List<BsonDocument> results = await context.ProfilePermissions.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Perfil de Usuário");
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

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.ProfilePermissions.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Perfil de Usuário não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Perfil de Usuário");
        }
    }   
    public async Task<ResponseApi<ProfilePermission?>> GetByIdAsync(string id)
    {
        try
        {
            ProfilePermission? employee = await context.ProfilePermissions.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(employee);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Perfil de Usuário");
        }
    } 
    public async Task<ResponseApi<dynamic?>> GetLoggedAsync(string id)
    {
        try
        {
            BsonDocument[] pipeline = [
                new("$match", new BsonDocument{
                    {"_id", new ObjectId(id)},
                    {"deleted", false}
                }),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),

                MongoUtil.Lookup("companies", ["$company"], ["$_id"], "_company", [["deleted", false]], 1),

                MongoUtil.Lookup("stores", ["$store"], ["$_id"], "_store", [["deleted", false]], 1),

                MongoUtil.Lookup("addresses", ["$id"], ["$parentId"], "_address", [["deleted", false]], 1),

                new("$addFields", new BsonDocument
                {
                    {"addressId", MongoUtil.First("_address._id")},
                    {"street",  MongoUtil.First("_address.street")},
                    {"number", MongoUtil.First("_address.number") },
                    {"complement", MongoUtil.First("_address.complement") },
                    {"neighborhood", MongoUtil.First("_address.neighborhood") },
                    {"city", MongoUtil.First("_address.city") },
                    {"state", MongoUtil.First("_address.state") },
                    {"zipCode", MongoUtil.First("_address.zipCode") },
                    {"parent", MongoUtil.First("_address.parent") },
                    {"parentId", MongoUtil.First("_address.parentId") },
                    {"logoCompany", MongoUtil.First("_company.photo") },
                    {"nameCompany", MongoUtil.First("_company.tradeName") },
                    {"nameStore", MongoUtil.First("_store.tradeName") },
                }),

                new("$addFields", new BsonDocument
                {
                    {"address", new BsonDocument
                        {
                            {"id", MongoUtil.ToString("$addressId")},
                            {"street",  MongoUtil.ValidateNull("street", "")},
                            {"number", MongoUtil.ValidateNull("number", "") },
                            {"complement", MongoUtil.ValidateNull("complement", "") },
                            {"neighborhood", MongoUtil.ValidateNull("neighborhood", "") },
                            {"city", MongoUtil.ValidateNull("city", "") },
                            {"state", MongoUtil.ValidateNull("state", "") },
                            {"zipCode", MongoUtil.ValidateNull("zipCode", "") },
                            {"parent", MongoUtil.ValidateNull("parent", "") },
                            {"parentId", MongoUtil.ValidateNull("parentId", "") },
                        }
                    }
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"name", 1},
                    {"email", 1},
                    {"modules", 1},
                    {"admin", 1},
                    {"blocked", 1},
                    {"photo", 1},
                    {"phone", 1},
                    {"whatsapp", 1},
                    {"logoCompany", MongoUtil.ValidateNull("logoCompany", "")},
                    {"nameCompany", MongoUtil.ValidateNull("nameCompany", "")},
                    {"nameStore", MongoUtil.ValidateNull("nameStore", "")},
                    {"address", 1}
                }),
            ];

            BsonDocument? response = await context.ProfilePermissions.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Usuário não encontrado") : new(result);
        }
        catch(Exception e)
        {
            return new(null, 500, e.Message); ;
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<ProfilePermission> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$sort", pagination.PipelineSort),
                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                new("$match", pagination.PipelineFilter),
                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", 1}, 
                    {"code", 1}, 
                    {"name", 1} 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.ProfilePermissions.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Empresas");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<ProfilePermission> pagination)
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

        List<BsonDocument> results = await context.ProfilePermissions.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    public async Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId, string storeId)
    {
        try
        {
            long code = await context.ProfilePermissions.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Próximo Código");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<ProfilePermission?>> CreateAsync(ProfilePermission employee)
    {
        try
        {
            await context.ProfilePermissions.InsertOneAsync(employee);

            return new(employee, 201, "Perfil de Usuário criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Perfil de Usuário");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<ProfilePermission?>> UpdateAsync(ProfilePermission employee)
    {
        try
        {
            await context.ProfilePermissions.ReplaceOneAsync(x => x.Id == employee.Id, employee);

            return new(employee, 201, "Perfil de Usuário atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Perfil de Usuário");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<ProfilePermission>> DeleteAsync(string id)
    {
        try
        {
            ProfilePermission? employee = await context.ProfilePermissions.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(employee is null) return new(null, 404, "Perfil de Usuário não encontrado");
            employee.Deleted = true;
            employee.DeletedAt = DateTime.UtcNow;

            await context.ProfilePermissions.ReplaceOneAsync(x => x.Id == id, employee);

            return new(employee, 204, "Perfil de Usuário excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Perfil de Usuário");
        }
    }
    #endregion
}
}