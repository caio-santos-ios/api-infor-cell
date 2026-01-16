using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class CategoryService(ICategoryRepository repository, IMapper _mapper) : ICategoryService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Category> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Categoryts = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Categoryts.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Categoryt = await repository.GetByIdAggregateAsync(id);
            if(Categoryt.Data is null) return new(null, 404, "Categoria não encontrada");
            return new(Categoryt.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Category?>> CreateAsync(CreateCategoryDTO request)
    {
        try
        {
            Category category = _mapper.Map<Category>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Company, request.Store);
            category.Code = code.Data.ToString().PadLeft(6, '0');
            ResponseApi<Category?> response = await repository.CreateAsync(category);

            if(response.Data is null) return new(null, 400, "Falha ao criar Categoria.");
            return new(response.Data, 201, "Categoria criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Category?>> UpdateAsync(UpdateCategoryDTO request)
    {
        try
        {
            ResponseApi<Category?> categoryResponse = await repository.GetByIdAsync(request.Id);
            if(categoryResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            Category category = _mapper.Map<Category>(request);
            category.UpdatedAt = DateTime.UtcNow;
            category.CreatedAt = categoryResponse.Data.CreatedAt;

            ResponseApi<Category?> response = await repository.UpdateAsync(category);
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
    public async Task<ResponseApi<Category>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Category> Category = await repository.DeleteAsync(id);
            if(!Category.IsSuccess) return new(null, 400, Category.Message);
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