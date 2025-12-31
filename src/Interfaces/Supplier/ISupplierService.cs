using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface ISupplierService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
    Task<ResponseApi<Supplier?>> CreateAsync(CreateSupplierDTO request);
    Task<ResponseApi<Supplier?>> UpdateAsync(UpdateSupplierDTO request);
    Task<ResponseApi<Supplier>> DeleteAsync(string id);
}
}