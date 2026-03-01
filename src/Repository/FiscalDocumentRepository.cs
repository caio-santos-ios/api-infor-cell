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
    public class FiscalDocumentRepository(AppDbContext context) : IFiscalDocumentRepository
    {
        public async Task<ResponseApi<FiscalDocument?>> GetByOriginAsync(string originId, string originType)
        {
            try
            {
                FiscalDocument? doc = await context.FiscalDocuments
                    .Find(x => x.OriginId == originId && x.OriginType == originType && !x.Deleted)
                    .SortByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();
                return new(doc);
            }
            catch { return new(null, 500, "Falha ao buscar documento fiscal"); }
        }

        public async Task<ResponseApi<FiscalDocument?>> GetByIdAsync(string id)
        {
            try
            {
                FiscalDocument? doc = await context.FiscalDocuments
                    .Find(x => x.Id == id && !x.Deleted)
                    .FirstOrDefaultAsync();
                return new(doc);
            }
            catch { return new(null, 500, "Falha ao buscar documento fiscal"); }
        }

        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<FiscalDocument> pagination)
        {
            try
            {
                List<BsonDocument> pipeline =
                [
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                    new("$match", pagination.PipelineFilter),
                    new("$project", new BsonDocument { { "_id", 0 } }),
                    new("$sort", pagination.PipelineSort),
                ];
                List<BsonDocument> results = await context.FiscalDocuments.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch { return new(null, 500, "Falha ao buscar documentos fiscais"); }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<FiscalDocument> pagination)
        {
            List<BsonDocument> pipeline =
            [
                new("$addFields", new BsonDocument { { "id", new BsonDocument("$toString", "$_id") } }),
                new("$match", pagination.PipelineFilter),
            ];
            List<BsonDocument> results = await context.FiscalDocuments.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Count;
        }

        public async Task<ResponseApi<FiscalDocument?>> CreateAsync(FiscalDocument doc)
        {
            try
            {
                await context.FiscalDocuments.InsertOneAsync(doc);
                return new(doc, 201, "Documento fiscal criado");
            }
            catch { return new(null, 500, "Falha ao criar documento fiscal"); }
        }

        public async Task<ResponseApi<FiscalDocument?>> UpdateAsync(FiscalDocument doc)
        {
            try
            {
                doc.UpdatedAt = DateTime.UtcNow;
                await context.FiscalDocuments.ReplaceOneAsync(x => x.Id == doc.Id, doc);
                return new(doc, 200, "Documento fiscal atualizado");
            }
            catch { return new(null, 500, "Falha ao atualizar documento fiscal"); }
        }

        public async Task<ResponseApi<FiscalEvent?>> CreateEventAsync(FiscalEvent evt)
        {
            try
            {
                await context.FiscalEvents.InsertOneAsync(evt);
                return new(evt, 201, "Evento fiscal registrado");
            }
            catch { return new(null, 500, "Falha ao registrar evento fiscal"); }
        }

        public async Task<ResponseApi<FiscalConfig?>> GetConfigByStoreAsync(string storeId)
        {
            try
            {
                FiscalConfig? config = await context.FiscalConfigs
                    .Find(x => x.Store == storeId && !x.Deleted)
                    .FirstOrDefaultAsync();
                return new(config);
            }
            catch { return new(null, 500, "Falha ao buscar configuração fiscal"); }
        }

        public async Task<ResponseApi<FiscalConfig?>> SaveConfigAsync(FiscalConfig config)
        {
            try
            {
                FiscalConfig? existing = await context.FiscalConfigs
                    .Find(x => x.Store == config.Store && !x.Deleted)
                    .FirstOrDefaultAsync();

                if (existing is null)
                {
                    await context.FiscalConfigs.InsertOneAsync(config);
                    return new(config, 201, "Configuração fiscal salva");
                }

                config.Id = existing.Id;
                config.UpdatedAt = DateTime.UtcNow;
                await context.FiscalConfigs.ReplaceOneAsync(x => x.Id == existing.Id, config);
                return new(config, 200, "Configuração fiscal atualizada");
            }
            catch { return new(null, 500, "Falha ao salvar configuração fiscal"); }
        }

        /// <summary>Atomicamente incrementa e retorna o próximo número de nota (evita concorrência)</summary>
        public async Task<long> GetNextNumberAsync(string storeId, int model)
        {
            string field = model == 55 ? "nextNumberNfe" : "nextNumberNfce";

            FilterDefinition<FiscalConfig> filter = Builders<FiscalConfig>.Filter
                .Where(x => x.Store == storeId && !x.Deleted);

            UpdateDefinition<FiscalConfig> update = Builders<FiscalConfig>.Update
                .Inc(field, 1L);

            FindOneAndUpdateOptions<FiscalConfig> options = new()
            {
                ReturnDocument = ReturnDocument.Before,
                IsUpsert = false
            };

            FiscalConfig? config = await context.FiscalConfigs.FindOneAndUpdateAsync(filter, update, options);
            return model == 55 ? config?.NextNumberNfe ?? 1 : config?.NextNumberNfce ?? 1;
        }
    }
}