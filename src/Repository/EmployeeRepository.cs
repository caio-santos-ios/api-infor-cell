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
    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Employee> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                MongoUtil.Lookup("profile_permissions", ["$type"], ["$_id"], "_permissions", [["deleted", false]], 1),

                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"typeName", MongoUtil.First("_permissions.name")},
                    {"teste", "$_permissions"},
                    {"name", 1},
                    {"email", 1},
                    {"cpf", 1},
                    {"dateOfBirth", 1},
                    {"createdAt", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Employees.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Profissionais");
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
                    {"_address", 0},
                }),
            ];

            BsonDocument? response = await context.Employees.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Profissionais não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Profissionais");
        }
    }   
    public async Task<ResponseApi<Employee?>> GetByIdAsync(string id)
    {
        try
        {
            Employee? employee = await context.Employees.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(employee);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Profissionais");
        }
    } 
    public async Task<ResponseApi<Employee?>> GetByUserIdAsync(string userId)
    {
        try
        {
            Employee? employee = await context.Employees.Find(x => x.UserId == userId && !x.Deleted).FirstOrDefaultAsync();
            return new(employee);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Profissionais");
        }
    } 
    // public async Task<ResponseApi<Employee?>> GetByEmailAsync(string email, string id)
    // {
    //     try
    //     {
    //         Employee? employee = new();
    //         if(!string.IsNullOrEmpty(id))
    //         {
    //             employee = await context.Employees.Find(x => x.Email == email && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
    //         }
    //         else
    //         {
    //             employee = await context.Employees.Find(x => x.Email == email && !x.Deleted).FirstOrDefaultAsync();
    //         };

    //         return new(employee);
    //     }
    //     catch
    //     {
    //         return new(null, 500, "Falha ao buscar Profissionais");
    //     }
    // }
    public async Task<ResponseApi<Employee?>> GetByCpfAsync(string cpf, string id)
    {
        try
        {
            Employee? employee = new();
            if(!string.IsNullOrEmpty(id))
            {
                employee = await context.Employees.Find(x => x.Cpf == cpf && x.Id != id && !x.Deleted).FirstOrDefaultAsync();
            }
            else
            {
                employee = await context.Employees.Find(x => x.Cpf == cpf && !x.Deleted).FirstOrDefaultAsync();
            };

            return new(employee);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Profissionais");
        }
    }
    // public async Task<ResponseApi<Employee?>> GetByCodeAccessAsync(string codeAccess)
    // {
    //     try
    //     {
    //         Employee? employee = await context.Employees.Find(x => x.CodeAccess == codeAccess && !x.ValidatedAccess && !x.Deleted).FirstOrDefaultAsync();
    //         return new(employee);
    //     }
    //     catch
    //     {
    //         return new(null, 500, "Falha ao buscar usuário");
    //     }
    // }
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

            BsonDocument? response = await context.Employees.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Usuário não encontrado") : new(result);
        }
        catch(Exception e)
        {
            return new(null, 500, e.Message); ;
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Employee> pagination)
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

        List<BsonDocument> results = await context.Employees.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    public async Task<ResponseApi<List<User>>> GetSellersAsync(string planId, string companyId, string storeId)
    {
        try
        {
            List<User> users = await context.Users.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId && !x.Deleted && x.Admin && x.Master).ToListAsync();
            // List<Employee> employees = await context.Employees.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId && !x.Deleted).ToListAsync();
            List<User> sellers = [];

            // foreach (Employee employee in employees)
            // {
            //     Module? moduleCommercial = employee.Modules.Where(m => m.Code == "C").FirstOrDefault();
            //     if(moduleCommercial is not null)
            //     {
            //         Routine? routineSalerOrder = moduleCommercial.Routines.Where(r => r.Code == "C1").FirstOrDefault();
            //         if(routineSalerOrder is not null)
            //         {
            //             if(routineSalerOrder.Permissions.Create && routineSalerOrder.Permissions.Update && routineSalerOrder.Permissions.Read)
            //             {
            //                 sellers.Add(new () 
            //                 {
            //                     Id = employee.Id,
            //                     Name = employee.Name
            //                 });
            //             }
            //         };
            //     };
            // };

            foreach (User user in users)
            {
                sellers.Add(new () {
                    Id = user.Id,
                    Name = user.Name
                });
            }

            return new(sellers);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Vendedores");
        }
    } 
    public async Task<ResponseApi<List<User>>> GetTechniciansAsync(string planId, string companyId, string storeId)
    {
        try
        {
            List<User> users = await context.Users.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId && !x.Deleted && x.Admin && x.Master).ToListAsync();
            // List<Employee> employees = await context.Employees.Find(x => x.Plan == planId && x.Company == companyId && x.Store == storeId && !x.Deleted).ToListAsync();
            List<User> sellers = [];

            // foreach (Employee employee in employees)
            // {
            //     Module? moduleCommercial = employee.Modules.Where(m => m.Code == "D").FirstOrDefault();
            //     if(moduleCommercial is not null)
            //     {
            //         Routine? routineSalerOrder = moduleCommercial.Routines.Where(r => r.Code == "D1").FirstOrDefault();
            //         if(routineSalerOrder is not null)
            //         {
            //             if(routineSalerOrder.Permissions.Create && routineSalerOrder.Permissions.Update && routineSalerOrder.Permissions.Read)
            //             {
            //                 sellers.Add(new () 
            //                 {
            //                     Id = employee.Id,
            //                     Name = employee.Name
            //                 });
            //             }
            //         };
            //     };
            // };

            foreach (User user in users)
            {
                sellers.Add(new () {
                    Id = user.Id,
                    Name = user.Name
                });
            }

            return new(sellers);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Técnicos");
        }
    } 
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Employee?>> CreateAsync(Employee employee)
    {
        try
        {
            await context.Employees.InsertOneAsync(employee);

            return new(employee, 201, "Profissionais criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Profissionais");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Employee?>> UpdateAsync(Employee employee)
    {
        try
        {
            await context.Employees.ReplaceOneAsync(x => x.Id == employee.Id, employee);

            return new(employee, 201, "Profissionais atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Profissionais");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Employee>> DeleteAsync(string id)
    {
        try
        {
            Employee? employee = await context.Employees.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(employee is null) return new(null, 404, "Profissionais não encontrado");
            employee.Deleted = true;
            employee.DeletedAt = DateTime.UtcNow;

            await context.Employees.ReplaceOneAsync(x => x.Id == id, employee);

            return new(employee, 204, "Profissionais excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Profissionais");
        }
    }
    #endregion
}
}