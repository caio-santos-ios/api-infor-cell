using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<PurchaseOrder?>> CreateAsync(CreatePurchaseOrderDTO request);
        Task<ResponseApi<PurchaseOrder?>> UpdateAsync(UpdatePurchaseOrderDTO request);
        Task<ResponseApi<PurchaseOrder?>> UpdateApprovalAsync(UpdatePurchaseOrderDTO request);
        Task<ResponseApi<PurchaseOrder>> DeleteAsync(string id);
    }
}