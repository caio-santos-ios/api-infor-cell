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
    public class PlanRepository(AppDbContext context) : IPlanRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$match", pagination.PipelineFilter),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0}, 
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Planos");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Plan> pagination)
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
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Planos");
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

                BsonDocument? response = await context.Plans.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Plano não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano");
            }
        }       
        public async Task<ResponseApi<Plan?>> GetByIdAsync(string id)
        {
            try
            {
                Plan? address = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(address);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano");
            }
        }        
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination)
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
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Plans.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Plan?>> CreateAsync(Plan address)
        {
            try
            {
                await context.Plans.InsertOneAsync(address);

                return new(address, 201, "Plano criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Plano");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Plan?>> UpdateAsync(Plan address)
        {
            try
            {
                await context.Plans.ReplaceOneAsync(x => x.Id == address.Id, address);

                return new(address, 201, "Plano atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Plano");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Plan>> DeleteAsync(string id)
        {
            try
            {
                Plan? address = await context.Plans.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(address is null) return new(null, 404, "Plano não encontrado");
                address.Deleted = true;
                address.DeletedAt = DateTime.UtcNow;

                await context.Plans.ReplaceOneAsync(x => x.Id == id, address);

                return new(address, 204, "Plano excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Plano");
            }
        }
        #endregion
    }
}