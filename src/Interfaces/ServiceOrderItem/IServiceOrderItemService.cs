using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IServiceOrderItemService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<ServiceOrderItem?>> CreateAsync(CreateServiceOrderItemDTO request);
    Task<ResponseApi<ServiceOrderItem?>> UpdateAsync(UpdateServiceOrderItemDTO request);
    //Task<ResponseApi<Product?>> SavePhotoProfileAsync(SaveProductPhotoDTO request);
    Task<ResponseApi<ServiceOrderItem>> DeleteAsync(string id);
}
}