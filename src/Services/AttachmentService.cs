using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class AttachmentService(IAttachmentRepository attachmentRepository, IMapper _mapper, IWebHostEnvironment env) : IAttachmentService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Attachment> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> attachments = await attachmentRepository.GetAllAsync(pagination);
            int count = await attachmentRepository.GetCountDocumentsAsync(pagination);
            return new(attachments.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    
    public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> attachment = await attachmentRepository.GetByIdAggregateAsync(id);
            if(attachment.Data is null) return new(null, 404, "Anexo não encontrado");
            return new(attachment.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Attachment?>> CreateAsync(CreateAttachmentDTO request)
    {
        try
        {
            if(request.File is null) return new(null, 400, "Arquivo é obrigatório.");

            Attachment attachment = _mapper.Map<Attachment>(request);
            ResponseApi<Attachment?> response = await attachmentRepository.CreateAsync(attachment);

            string webRoot = env.WebRootPath;

            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            };

            string uploadPath = Path.Combine(webRoot, "uploads", request.Parent);

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File!.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            attachment.Uri = Path.Combine("uploads", request.Parent, fileName);
            await attachmentRepository.UpdateAsync(attachment);

            if(response.Data is null) return new(null, 400, "Falha ao criar Anexo.");
            return new(response.Data, 201, "Anexo criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Attachment?>> UpdateAsync(UpdateAttachmentDTO request)
    {
        try
        {
            ResponseApi<Attachment?> attachmentResponse = await attachmentRepository.GetByIdAsync(request.Id);
            if(attachmentResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Attachment attachment = _mapper.Map<Attachment>(request);
            attachment.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Attachment?> response = await attachmentRepository.UpdateAsync(attachment);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            if(request.File is not null) {
                string webRoot = env.WebRootPath;

                if (string.IsNullOrEmpty(webRoot))
                {
                    webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                };

                string uploadPath = Path.Combine(webRoot, "uploads", request.Parent);

                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File!.FileName)}";
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                attachment.Uri = Path.Combine("uploads", request.Parent, fileName);
                await attachmentRepository.UpdateAsync(attachment);
            };

            return new(response.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Attachment>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Attachment> attachment = await attachmentRepository.DeleteAsync(id);
            if(!attachment.IsSuccess) return new(null, 400, attachment.Message);
            return new(null, 204, "Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}