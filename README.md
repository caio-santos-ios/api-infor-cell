# AVISO IMPORTANTE, AS MODELS SEMPRE ESTARAM CRIADAS. DEVE COMEÇAR PELO 2° PASSO 

1. Criar a Model (Entidade)
A Model representa a tabela no banco de dados. Ela deve ser criada na pasta src/Models/.

public class Store 
{
    [BsonElement("companyId")]
    public string CompanyId { get; set; } = string.Empty; 
}

2. Criar os DTOs (Data Transfer Objects)
Nunca exponha sua Model diretamente. Crie DTOs para entrada (Request) na pasta src/Shared/DTOs/.

## AVISO IMPORTANTE, OS CAMPOS SERÁ SEMPRE OS MESMOS DAS MODELS, RETIRA O ID DO CREATE E ADICIONA NO UPDATE
public class CreateStoreDTO : ModelBase
{
    public string CompanyId { get; set; } = string.Empty; 
    
    public string Document { get; set; } = string.Empty; 

    public string CorporateName { get; set; } = string.Empty; 

    public string TradeName { get; set; } = string.Empty; 

    public string StateRegistration { get; set; } = string.Empty; 

    public string MunicipalRegistration { get; set; } = string.Empty; 

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Whatsapp { get; set; } = string.Empty;

    public string Photo { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;
}

public class UpdateStoreDTO : ModelBase
{
    public string Id { get; set; } = string.Empty;

    public string CompanyId { get; set; } = string.Empty; 
    
    public string Document { get; set; } = string.Empty; 

    public string CorporateName { get; set; } = string.Empty; 

    public string TradeName { get; set; } = string.Empty; 

    public string StateRegistration { get; set; } = string.Empty; 

    public string MunicipalRegistration { get; set; } = string.Empty; 

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Whatsapp { get; set; } = string.Empty;

    public string Photo { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;
}

4. Criar collection(Tabela)
Adicione a regra no seu src/Configuration/AppDbContext.cs
# AVISO IMPORTANTE, DEVE SER CRIADO DEPOIS DO ULTIMO MAPEAMENTO
CreateMap<CreateStoreDTO, Store>().ReverseMap();
CreateMap<UpdateStoreDTO, Store>().ReverseMap();

5. Configurar o Mapeamento
Adicione a regra no seu src/Configuration/MappingProfile.cs
# AVISO IMPORTANTE, DEVE SER CRIADO DEPOIS DA ULTIMA COLLECTION
public IMongoCollection<Store> Stores
{
    get { return Database.GetCollection<Store>("stores"); }
}

6. Criar Interfaces
Adicione a regra no seu src/Interfaces/Store/.

Criar arquivo IStoreRepository.cs
public interface IStoreRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Store> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Store?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<Store> pagination);
    Task<ResponseApi<Store?>> CreateAsync(Store address);
    Task<ResponseApi<Store?>> UpdateAsync(Store address);
    Task<ResponseApi<Store>> DeleteAsync(string id);
}

Criar arquivo IStoreService.cs
public interface IStoreService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Store?>> CreateAsync(CreateStoreDTO request);
    Task<ResponseApi<Store?>> UpdateAsync(UpdateStoreDTO request);
    Task<ResponseApi<Store?>> SavePhotoProfileAsync(SaveStorePhotoDTO request);
    Task<ResponseApi<Store>> DeleteAsync(string id);
}

7. Criar Repository
Adicione a regra no seu src/Repository.

Criar arquivo StoreRepository.cs
public class StoreRepository(AppDbContext context) : IStoreRepository
{
    #region READ
    public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Store> pagination)
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

            List<BsonDocument> results = await context.Stores.Aggregate<BsonDocument>(pipeline).ToListAsync();
            List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
            return new(list);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
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

            BsonDocument? response = await context.Stores.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            dynamic? result = response is null ? null : BsonSerializer.Deserialize<dynamic>(response);
            return result is null ? new(null, 404, "Loja não encontrado") : new(result);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<Store?>> GetByIdAsync(string id)
    {
        try
        {
            Store? address = await context.Stores.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            return new(address);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<int> GetCountDocumentsAsync(PaginationUtil<Store> pagination)
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

        List<BsonDocument> results = await context.Stores.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).Count();
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Store?>> CreateAsync(Store address)
    {
        try
        {
            await context.Stores.InsertOneAsync(address);

            return new(address, 201, "Loja criada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");  
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Store?>> UpdateAsync(Store address)
    {
        try
        {
            await context.Stores.ReplaceOneAsync(x => x.Id == address.Id, address);

            return new(address, 201, "Loja atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Store>> DeleteAsync(string id)
    {
        try
        {
            Store? address = await context.Stores.Find(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
            if(address is null) return new(null, 404, "Loja não encontrado");
            address.Deleted = true;
            address.DeletedAt = DateTime.UtcNow;

            await context.Stores.ReplaceOneAsync(x => x.Id == id, address);

            return new(address, 204, "Loja excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
}

8. Criar Service
Adicione a regra no seu src/Service.

Criar arquivo StoreService.cs
public class StoreService(IStoreService repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IStoreService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Store> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> stores = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(stores.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> store = await repository.GetByIdAggregateAsync(id);
            if(store.Data is null) return new(null, 404, "Empresa não encontrada");
            return new(store.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Store?>> CreateAsync(CreateStoreDTO request)
    {
        try
        {
            Store store = _mapper.Map<Store>(request);
            ResponseApi<Store?> response = await repository.CreateAsync(store);

            if(response.Data is null) return new(null, 400, "Falha ao criar Empresa.");
            return new(response.Data, 201, "Empresa criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Store?>> UpdateAsync(UpdateStoreDTO request)
    {
        try
        {
            ResponseApi<Store?> storeResponse = await repository.GetByIdAsync(request.Id);
            if(storeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Store store = _mapper.Map<Store>(request);
            store.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Store?> response = await repository.UpdateAsync(store);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Store?>> SavePhotoProfileAsync(SaveStorePhotoDTO request)
    {
        try
        {
            if (request.Photo == null || request.Photo.Length == 0) return new(null, 400, "Falha ao salvar foto de perfil");

            ResponseApi<Store?> user = await repository.GetByIdAsync(request.Id);
            if(user.Data is null) return new(null, 404, "Falha ao salvar foto de perfil");

            var tempPath = Path.GetTempFileName();

            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                request.Photo.CopyTo(stream);
            }

            string uriPhoto = await cloudinaryHandler.UploadAttachment("store", request.Photo);
            user.Data.UpdatedAt = DateTime.UtcNow;
            user.Data.Photo = uriPhoto;

            ResponseApi<Store?> response = await repository.UpdateAsync(user.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao salvar logo");
            return new(new () { Photo = response.Data!.Photo }, 201, "Logo salvo com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Store>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Store> store = await repository.DeleteAsync(id);
            if(!store.IsSuccess) return new(null, 400, store.Message);
            return new(null, 204, "Excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}

9. Configurar os Serviços
Adicione a regra no seu src/Configuration/Build.cs
# AVISO IMPORTANTE, DEVE SER ADICIONADO DEPOIS DA ULTIMA SERVICE DENTRO DO METODO AddBuilderServices
builder.Services.AddTransient<IStoreService, StoreService>();
builder.Services.AddTransient<IStoreRepository, StoreRepository>();

10. Criar Controller
Adicione a regra no seu src/Controller.

Criar arquivo StoreController.cs

[Route("api/stores")]
[ApiController]
public class StoreController(IStoreService service) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
        return StatusCode(response.StatusCode, new { response.Result });
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
        return StatusCode(response.StatusCode, new { response.Result });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStoreDTO body)
    {
        if (body == null) return BadRequest("Dados inválidos.");

        ResponseApi<Store?> response = await service.CreateAsync(body);

        return StatusCode(response.StatusCode, new { response.Result });
    }
    
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStoreDTO body)
    {
        if (body == null) return BadRequest("Dados inválidos.");

        ResponseApi<Store?> response = await service.UpdateAsync(body);

        return StatusCode(response.StatusCode, new { response.Result });
    }

    [Authorize]
    [HttpPut("logo")]
    public async Task<IActionResult> SavePhotoProfileAsync([FromForm] SaveStorePhotoDTO body)
    {
        ResponseApi<Store?> response = await service.SavePhotoProfileAsync(body);
        return StatusCode(response.StatusCode, new { response.Message, response.Result });
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        ResponseApi<Store> response = await service.DeleteAsync(id);

        return StatusCode(response.StatusCode, new { response.Result });
    }
}