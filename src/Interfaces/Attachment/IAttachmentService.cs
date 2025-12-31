using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;

namespace api_infor_cell.src.Interfaces
{
   public interface IAttachmentService
{
    Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request);
    Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
    Task<ResponseApi<Attachment?>> CreateAsync(CreateAttachmentDTO request);
    Task<ResponseApi<Attachment?>> UpdateAsync(UpdateAttachmentDTO request);
    Task<ResponseApi<Attachment>> DeleteAsync(string id);
}
}