using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.Utils;

namespace api_infor_cell.src.Interfaces
{
public interface IPaymentMethodRepository
{
    Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<PaymentMethod> pagination);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<PaymentMethod?>> GetByIdAsync(string id);
    Task<int> GetCountDocumentsAsync(PaginationUtil<PaymentMethod> pagination);
    Task<ResponseApi<PaymentMethod?>> CreateAsync(PaymentMethod address);
    Task<ResponseApi<PaymentMethod?>> UpdateAsync(PaymentMethod address);
    Task<ResponseApi<PaymentMethod>> DeleteAsync(string id);
}
}
