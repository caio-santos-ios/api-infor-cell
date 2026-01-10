using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class BrandService(IBrandRepository repository, IMapper _mapper) : IBrandService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Brand> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Brands = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Brands.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Brand> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> brands = await repository.GetAllAsync(pagination);
            return new(brands.Data);
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
            ResponseApi<dynamic?> Brand = await repository.GetByIdAggregateAsync(id);
            if(Brand.Data is null) return new(null, 404, "Marca não encontrada");
            return new(Brand.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Brand?>> CreateAsync(CreateBrandDTO request)
    {
        try
        {
            Brand brand = _mapper.Map<Brand>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Company, request.Store);
            brand.Code = code.Data.ToString().PadLeft(6, '0');
            ResponseApi<Brand?> response = await repository.CreateAsync(brand);

            if(response.Data is null) return new(null, 400, "Falha ao criar Marca.");
            return new(response.Data, 201, "Marca criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Brand?>> UpdateAsync(UpdateBrandDTO request)
    {
        try
        {
            ResponseApi<Brand?> brandResponse = await repository.GetByIdAsync(request.Id);
            if(brandResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Brand brand = _mapper.Map<Brand>(request);
            brand.UpdatedAt = DateTime.UtcNow;
            brand.CreatedAt = brandResponse.Data.CreatedAt;

            ResponseApi<Brand?> response = await repository.UpdateAsync(brand);
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
    public async Task<ResponseApi<Brand>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Brand> Brand = await repository.DeleteAsync(id);
            if(!Brand.IsSuccess) return new(null, 400, Brand.Message);
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