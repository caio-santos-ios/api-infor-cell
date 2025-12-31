using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
    public interface IGenericTableService
    {
        Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetByTableAggregateAsync(string table);
        Task<ResponseApi<GenericTable?>> CreateAsync(CreateGenericTableDTO user);
        Task<ResponseApi<GenericTable?>> UpdateAsync(UpdateGenericTableDTO request);
        Task<ResponseApi<GenericTable>> DeleteAsync(string id);
        Task<ResponseApi<GenericTable>> DeleteByTableAsync(string table);
    }
}