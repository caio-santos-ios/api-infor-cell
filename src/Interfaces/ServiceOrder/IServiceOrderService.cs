using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IServiceOrderService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei);
        Task<ResponseApi<ServiceOrder?>> CreateAsync(CreateServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder?>> UpdateAsync(UpdateServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder?>> CloseAsync(CloseServiceOrderDTO request);
        Task<ResponseApi<ServiceOrder>> DeleteAsync(string id);
    }
}