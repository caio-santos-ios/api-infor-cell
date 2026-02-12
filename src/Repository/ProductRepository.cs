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
    public class ProductRepository(AppDbContext context) : IProductRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Product> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                MongoUtil.Lookup("models", ["$modelId"], ["$_id"], "_model", [["deleted", false]], 1),
                MongoUtil.Lookup("categories", ["$categoryId"], ["$_id"], "_category", [["deleted", false]], 1),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"code", 1},
                    {"name", 1},
                    {"description", 1},
                    {"createdAt", 1},
                    {"modelName", MongoUtil.First("_model.name")},
                    {"categoryName", MongoUtil.First("_category.name")},
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Produto");
        }
    }    
    public async Task<ResponseApi<List<dynamic>>> GetAutocompleteAsync(PaginationUtil<Product> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                
                MongoUtil.Lookup("models", ["$modelId"], ["$_id"], "_model", [["deleted", false]], 1),
                MongoUtil.Lookup("categories", ["$categoryId"], ["$_id"], "_category", [["deleted", false]], 1),
                MongoUtil.Lookup("attachments", ["$id"], ["$parentId"], "_images", [["deleted", false]], 1),
                MongoUtil.Lookup("stock", ["$_id"], ["$productId"], "_stock", [["deleted", false]]),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                    {"id", 1},
                    {"code", 1},
                    {"name", 1},
                    {"description", 1},
                    {"createdAt", 1},
                    {"variations", 1},
                    {"variationsCode", 1},
                    {"price", 1},
                    {"hasSerial", 1},
                    {"hasVariations", 1},
                    {"modelName", MongoUtil.First("_model.name")},
                    {"categoryName", MongoUtil.First("_category.name")},
                    {"productName", MongoUtil.Concat(["$code", " - ", "$name"])},
                    {"image", MongoUtil.First("_images.uri")},
                    {"stock", new BsonDocument("$map", new BsonDocument 
                        {
                            {"input", "$_stock"},
                            {"as", "s"},
                            {"in", new BsonDocument 
                                {
                                    {"id", new BsonDocument("$toString", "$$s._id")},
                                    {"quantity", "$$s.quantity"},
                                    {"deleted", "$$s.deleted"},
                                    {"variations", "$$s.variations"}
                                }
                            }
                        })
                    }
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Produto");
        }
    }    
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Product> pagination)
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
                    {"hasSerial", 1},
                    {"variations", 1}
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Produto");
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
                
                // MongoUtil.Lookup("stock", ["$productId"], ["$_id"], "_stock", [["deleted", false]]),
                MongoUtil.Lookup("stock", ["$_id", "$store"], ["$productId", "$store"], "_stock", [["deleted", false]]),

                new("$addFields", new BsonDocument {
                    {"id", new BsonDocument("$toString", "$_id")},
                }),
                
                new("$addFields", new BsonDocument {
                    {"stock", new BsonDocument("$map", new BsonDocument 
                        {
                            {"input", "$_stock"},
                            {"as", "s"},
                            {"in", new BsonDocument 
                                {
                                    {"id", new BsonDocument("$toString", "$$s._id")},
                                    {"store", "$$s.store"},
                                    {"quantity", "$$s.quantity"},
                                    {"deleted", "$$s.deleted"},
                                    {"variations", "$$s.variations"}
                                }
                            }
                        })
                    },
                }),

                new("$project", new BsonDocument
                {
                    {"_id", 0},
                }),
            ];

            BsonDocument? response = await context.Products.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Produto não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Produto");
        }
    }
    public async Task<ResponseApi<Product?>> GetByIdAsync(string id)
    {
        try
        {
            Product? product = await context.Products.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(product);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Produto");
        }
    }
    public async Task<ResponseApi<long>> GetNextCodeAsync(string companyId, string storeId)
    {
        try
        {
            long code = await context.Products.Find(x => x.Company == companyId && x.Store == storeId).CountDocumentsAsync() + 1;
            return new(code);
        }
        catch
        {
            return new(0, 500, "Falha ao buscar Produto");
        }
    }
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Product> pagination)
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

        List<BsonDocument> results = await context.Products.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Product?>> CreateAsync(Product product)
    {
        try
        {
            await context.Products.InsertOneAsync(product);

            return new(product, 201, "Produto criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Produto");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Product?>> UpdateAsync(Product product)
    {
        try
        {
            await context.Products.ReplaceOneAsync(x => x.Id == product.Id, product);

            return new(product, 200, "Produto atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Produto");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Product>> DeleteAsync(string id)
    {
        try
        {
            Product? product = await context.Products.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(product is null) return new(null, 404, "Produto não encontrado");
            product.Deleted = true;
            product.DeletedAt = DateTime.UtcNow;

            await context.Products.ReplaceOneAsync(x => x.Id == id, product);

            return new(product, 204, "Produto excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Produto");
        }
    }
    #endregion
}
}