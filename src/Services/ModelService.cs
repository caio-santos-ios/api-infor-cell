using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ModelService(IModelRepository repository, IMapper _mapper) : IModelService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Model> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Models = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Models.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Model = await repository.GetByIdAggregateAsync(id);
            if(Model.Data is null) return new(null, 404, "Modelo não encontrada");
            return new(Model.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Model?>> CreateAsync(CreateModelDTO request)
    {
        try
        {
            Model model = _mapper.Map<Model>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Company, request.Store);
            model.Code = code.Data.ToString().PadLeft(6, '0');
            ResponseApi<Model?> response = await repository.CreateAsync(model);

            if(response.Data is null) return new(null, 400, "Falha ao criar Modelo.");
            return new(response.Data, 201, "Modelo criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Model?>> UpdateAsync(UpdateModelDTO request)
    {
        try
        {
            ResponseApi<Model?> modelResponse = await repository.GetByIdAsync(request.Id);
            if(modelResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Model model = _mapper.Map<Model>(request);
            model.UpdatedAt = DateTime.UtcNow;
            model.CreatedAt = modelResponse.Data.CreatedAt;

            ResponseApi<Model?> response = await repository.UpdateAsync(model);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
   
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Model>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Model> Model = await repository.DeleteAsync(id);
            if(!Model.IsSuccess) return new(null, 400, Model.Message);
            return new(null, 204, "Excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}