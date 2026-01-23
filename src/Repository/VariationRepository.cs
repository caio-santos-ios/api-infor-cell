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
    public class VariationRepository(AppDbContext context) : IVariationRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Variation> pagination)
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
                        {"createdAt", 1},
                        {"active", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Variations.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Variação");
            }
        }    
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Variation> pagination)
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
                        {"items", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Variations.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Variação");
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

                BsonDocument? response = await context.Variations.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
                return result is null ? new(null, 404, "Variação não encontrado") : new(result);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Variação");
            }
        }
        public async Task<ResponseApi<Variation?>> GetByIdAsync(string id)
        {
            try
            {
                Variation? variation = await context.Variations.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                return new(variation);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Variação");
            }
        }
        public async Task<ResponseApi<List<Variation>>> GetSerialExistedAsync(string serial)
        {
            try
            {
                List<Variation> variations = await context.Variations.Find(x => x.Items.Where(i => i.Serial.Where(s => s.Value == serial).Any()).Any() && !x.Deleted).ToListAsync();
                return new(variations);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Variação");
            }
        }
        public async Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId)
        {
            try
            {
                long code = await context.Variations.Find(x => x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
                return new(code);
            }
            catch
            {
                return new(0, 500, "Falha ao buscar Variação");
            }
        }
        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Variation> pagination)
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

            List<BsonDocument> results = await context.Variations.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Variation?>> CreateAsync(Variation variation)
        {
            try
            {
                await context.Variations.InsertOneAsync(variation);

                return new(variation, 201, "Variação criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao criar Variação");  
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Variation?>> UpdateAsync(Variation variation)
        {
            try
            {
                await context.Variations.ReplaceOneAsync(x => x.Id == variation.Id, variation);

                return new(variation, 200, "Variação atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Variação");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Variation>> DeleteAsync(string id)
        {
            try
            {
                Variation? variation = await context.Variations.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
                if(variation is null) return new(null, 404, "Variação não encontrado");
                variation.Deleted = true;
                variation.DeletedAt = DateTime.UtcNow;

                await context.Variations.ReplaceOneAsync(x => x.Id == id, variation);

                return new(variation, 204, "Variação excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Falha ao excluír Variação");
            }
        }
        #endregion
    }
}