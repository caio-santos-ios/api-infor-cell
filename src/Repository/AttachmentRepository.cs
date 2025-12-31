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
    public class AttachmentRepository(AppDbContext context) : IAttachmentRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Attachment> pagination)
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

            List<BsonDocument> results = await context.Attachments.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Anexos");
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

            BsonDocument? response = await context.Attachments.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Anexo não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Anexo");
        }
    }
    
    public async Task<ResponseApi<Attachment?>> GetByIdAsync(string id)
    {
        try
        {
            Attachment? attachment = await context.Attachments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(attachment);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Anexo");
        }
    }
    public async Task<ResponseApi<Attachment?>> GetByParentIdAsync(string parentId, string parent)
    {
        try
        {
            Attachment? attachment = await context.Attachments.Find(x => x.ParentId == parentId && x.Parent == parent && !x.Deleted).FirstOrDefaultAsync();
            return new(attachment);
        }
        catch
        {
            return new(null, 500, "Falha ao buscar Anexo");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Attachment> pagination)
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

        List<BsonDocument> results = await context.Attachments.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Attachment?>> CreateAsync(Attachment attachment)
    {
        try
        {
            await context.Attachments.InsertOneAsync(attachment);

            return new(attachment, 201, "Anexo criado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao criar Anexo");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Attachment?>> UpdateAsync(Attachment attachment)
    {
        try
        {
            await context.Attachments.ReplaceOneAsync(x => x.Id == attachment.Id, attachment);

            return new(attachment, 201, "Anexo atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Anexo");
        }
    }
    public async Task<ResponseApi<Attachment?>> UpdateParentAsync(Attachment attachment)
    {
        try
        {
            await context.Attachments.ReplaceOneAsync(x => x.ParentId == attachment.ParentId && x.Parent == attachment.Parent, attachment);

            return new(attachment, 201, "Anexo atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao atualizar Anexo");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Attachment>> DeleteAsync(string id)
    {
        try
        {
            Attachment? attachment = await context.Attachments.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(attachment is null) return new(null, 404, "Anexo não encontrado");
            attachment.Deleted = true;
            attachment.DeletedAt = DateTime.UtcNow;

            await context.Attachments.ReplaceOneAsync(x => x.Id == id, attachment);

            return new(attachment, 204, "Anexo excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Falha ao excluír Anexo");
        }
    }
    #endregion
}
}