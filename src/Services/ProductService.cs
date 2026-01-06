using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ProductService(IProductRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IProductService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Product> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Products = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Products.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Product = await repository.GetByIdAggregateAsync(id);
            if(Product.Data is null) return new(null, 404, "Loja não encontrada");
            return new(Product.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Product?>> CreateAsync(CreateProductDTO request)
    {
        try
        {
            Product Product = _mapper.Map<Product>(request);
            ResponseApi<Product?> response = await repository.CreateAsync(Product);

            if(response.Data is null) return new(null, 400, "Falha ao criar Loja.");
            return new(response.Data, 201, "Loja criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Product?>> UpdateAsync(UpdateProductDTO request)
    {
        try
        {
            ResponseApi<Product?> ProductResponse = await repository.GetByIdAsync(request.Id);
            if(ProductResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Product Product = _mapper.Map<Product>(request);
            Product.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Product?> response = await repository.UpdateAsync(Product);
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
    public async Task<ResponseApi<Product>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Product> Product = await repository.DeleteAsync(id);
            if(!Product.IsSuccess) return new(null, 400, Product.Message);
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