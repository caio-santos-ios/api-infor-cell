using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ISalesOrderItemService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<SalesOrderItem?>> CreateAsync(CreateSalesOrderItemDTO request);
    Task<ResponseApi<SalesOrderItem?>> UpdateAsync(UpdateSalesOrderItemDTO request);
    //Task<ResponseApi<SalesOrderItem?>> SavePhotoProfileAsync(SaveSalesOrderItemPhotoDTO request);
    Task<ResponseApi<SalesOrderItem>> DeleteAsync(string id);
}
}