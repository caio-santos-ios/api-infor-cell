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
    public class ChartOfAccountsRepository(AppDbContext context) : IChartOfAccountsRepository
    {
        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<ChartOfAccounts> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    MongoUtil.Lookup("chart_of_accounts", ["$parentId"], ["$_id"], "_parent", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument
                    {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"parentName", MongoUtil.First("_parent.name")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_parent", 0},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Plano de Contas");
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
                    MongoUtil.Lookup("chart_of_accounts", ["$parentId"], ["$_id"], "_parent", [["deleted", false]], 1),
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"parentName", MongoUtil.First("_parent.name")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"_parent", 0},
                    }),
                ];

                BsonDocument? result = await context.ChartOfAccounts.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                dynamic? data = result is not null ? BsonSerializer.Deserialize<dynamic>(result) : null;
                return new(data);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Conta");
            }
        }

        public async Task<ResponseApi<ChartOfAccounts?>> GetByIdAsync(string id)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.And(
                    Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(id)),
                    Builders<ChartOfAccounts>.Filter.Eq("deleted", false)
                );

                ChartOfAccounts? obj = await context.ChartOfAccounts.Find(filter).FirstOrDefaultAsync();
                return new(obj);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar Conta");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<ChartOfAccounts> pagination)
        {
            try
            {
                long count = await context.ChartOfAccounts.CountDocumentsAsync(pagination.Filter ?? Builders<ChartOfAccounts>.Filter.Empty);
                return (int)count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<ResponseApi<long>> GetNextCodeAsync(string planId, string companyId)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.And(
                    Builders<ChartOfAccounts>.Filter.Eq("plan", planId),
                    Builders<ChartOfAccounts>.Filter.Eq("company", companyId),
                    Builders<ChartOfAccounts>.Filter.Eq("deleted", false)
                );

                SortDefinition<ChartOfAccounts> sort = Builders<ChartOfAccounts>.Sort.Descending("code");
                ChartOfAccounts? lastRecord = await context.ChartOfAccounts.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();

                long nextCode = lastRecord is not null && long.TryParse(lastRecord.Code, out long code) ? code + 1 : 1;
                return new(nextCode);
            }
            catch
            {
                return new(1, 500, "Falha ao buscar próximo código");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<ChartOfAccounts?>> CreateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                await context.ChartOfAccounts.InsertOneAsync(chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null, 500, "Falha ao criar Conta");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<ChartOfAccounts?>> UpdateAsync(ChartOfAccounts chartOfAccounts)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(chartOfAccounts.Id));
                await context.ChartOfAccounts.ReplaceOneAsync(filter, chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null, 500, "Falha ao atualizar Conta");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<ChartOfAccounts>> DeleteAsync(string id)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.Eq("_id", new ObjectId(id));
                ChartOfAccounts? chartOfAccounts = await context.ChartOfAccounts.Find(filter).FirstOrDefaultAsync();

                if (chartOfAccounts is null)
                {
                    return new(null!, 404, "Conta não encontrada");
                }

                chartOfAccounts.Deleted = true;
                await context.ChartOfAccounts.ReplaceOneAsync(filter, chartOfAccounts);
                return new(chartOfAccounts);
            }
            catch
            {
                return new(null!, 500, "Falha ao deletar Conta");
            }
        }
        #endregion

        #region TREE
        public async Task<ResponseApi<List<dynamic>>> GetTreeAsync(string planId, string companyId)
        {
            try
            {
                FilterDefinition<ChartOfAccounts> filter = Builders<ChartOfAccounts>.Filter.And(
                    Builders<ChartOfAccounts>.Filter.Eq("plan", planId),
                    Builders<ChartOfAccounts>.Filter.Eq("company", companyId),
                    Builders<ChartOfAccounts>.Filter.Eq("deleted", false)
                );

                List<ChartOfAccounts> allAccounts = await context.ChartOfAccounts.Find(filter).ToListAsync();

                // Monta árvore hierárquica
                var accountsDict = allAccounts.ToDictionary(a => a.Id);
                var tree = new List<dynamic>();

                foreach (var account in allAccounts.Where(a => string.IsNullOrEmpty(a.ParentId)))
                {
                    tree.Add(BuildTree(account, accountsDict));
                }

                return new(tree);
            }
            catch
            {
                return new(null, 500, "Falha ao buscar árvore de contas");
            }
        }

        private dynamic BuildTree(ChartOfAccounts account, Dictionary<string, ChartOfAccounts> allAccounts)
        {
            var children = allAccounts.Values
                .Where(a => a.ParentId == account.Id)
                .Select(child => BuildTree(child, allAccounts))
                .ToList();

            return new
            {
                id = account.Id,
                code = account.Code,
                name = account.Name,
                type = account.Type,
                dreCategory = account.DreCategory,
                showInDre = account.ShowInDre,
                level = account.Level,
                isAnalytical = account.IsAnalytical,
                children
            };
        }
        #endregion
    }
}