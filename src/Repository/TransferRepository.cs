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
    public class TransferRepository(AppDbContext context) : ITransferRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Transfer> pagination)
    {
        try
        {
            List<BsonDocument> pipeline = new()
            {
                new("$match", pagination.PipelineFilter),
                new("$sort", pagination.PipelineSort),
                new("$skip", pagination.Skip),
                new("$limit", pagination.Limit),
                
                MongoUtil.Lookup("purchase_order_items", ["$purchaseOrderItemId"], ["$_id"], "_purchaseOrderItem", [["deleted", false]], 1),
                MongoUtil.Lookup("stores", ["$storeOriginId"], ["$_id"], "_storeOrigin", [["deleted", false]], 1),
                MongoUtil.Lookup("stores", ["$storeDestinationId"], ["$_id"], "_storeDestination", [["deleted", false]], 1),
                
                new("$addFields", new BsonDocument
                {
                    {"productId", MongoUtil.First("_purchaseOrderItem.productId")},
                }),

                MongoUtil.Lookup("products", ["$productId"], ["$_id"], "_product", [["deleted", false]], 1),

                new("$addFields", new BsonDocument
                {
                    {"id", new BsonDocument("$toString", "$_id")},
                    {"productName", MongoUtil.First("_product.name")},
                    {"storeOriginName", MongoUtil.First("_storeOrigin.tradeName")},
                    {"storeDestinationName", MongoUtil.First("_storeDestination.tradeName")},
                }),
                new("$project", new BsonDocument
                {
                    {"_id", 0}, 
                }),
                new("$sort", pagination.PipelineSort),
            };

            List<BsonDocument> results = await context.Transfers.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Transferências");
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

            BsonDocument? response = await context.Transfers.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Transferência não encontrada") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Transferência");
        }
    }
    
    public async Task<ResponseApi<Transfer?>> GetByIdAsync(string id)
    {
        try
        {
            Transfer? address = await context.Transfers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Transferência");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Transfer> pagination)
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

        List<BsonDocument> results = await context.Transfers.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Transfer?>> CreateAsync(Transfer address)
    {
        try
        {
            await context.Transfers.InsertOneAsync(address);

            return new(address, 201, "Transferência criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Transferência");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Transfer?>> UpdateAsync(Transfer address)
    {
        try
        {
            await context.Transfers.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Transferências atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Transferência");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Transfer>> DeleteAsync(string id)
    {
        try
        {
            Transfer? address = await context.Transfers.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Transferência não encontrada");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.Transfers.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Transferência excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Transferência");
        }
    }
    #endregion
}
}