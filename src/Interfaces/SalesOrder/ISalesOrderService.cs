using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ISalesOrderService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<SalesOrder?>> CreateAsync(CreateSalesOrderDTO request);
    Task<ResponseApi<SalesOrder?>> UpdateAsync(UpdateSalesOrderDTO request);
    //Task<ResponseApi<SalesOrder?>> SavePhotoProfileAsync(SaveSalesOrderPhotoDTO request);
    Task<ResponseApi<SalesOrder>> DeleteAsync(string id);
}
}