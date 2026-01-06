using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IProductService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Product?>> CreateAsync(CreateProductDTO request);
    Task<ResponseApi<Product?>> UpdateAsync(UpdateProductDTO request);
    //Task<ResponseApi<Product?>> SavePhotoProfileAsync(SaveProductPhotoDTO request);
    Task<ResponseApi<Product>> DeleteAsync(string id);
}
}