using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IPaymentMethodService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PaymentMethod?>> CreateAsync(CreatePaymentMethodDTO request);
    Task<ResponseApi<PaymentMethod?>> UpdateAsync(UpdatePaymentMethodDTO request);
    //Task<ResponseApi<PaymentMethod?>> SavePhotoProfileAsync(SavePaymentMethodPhotoDTO request);
    Task<ResponseApi<PaymentMethod>> DeleteAsync(string id);
}
}