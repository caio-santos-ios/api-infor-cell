using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ContactService(IContactRepository contactRepository, IMapper _mapper) : IContactService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Contact> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> contacts = await contactRepository.GetAllAsync(pagination);
            int count = await contactRepository.GetCountDocumentsAsync(pagination);
            return new(contacts.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> contact = await contactRepository.GetByIdAggregateAsync(id);
            if(contact.Data is null) return new(null, 404, "Contato não encontrado");
            return new(contact.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Contact?>> CreateAsync(CreateContactDTO request)
    {
        try
        {
            Contact contact = _mapper.Map<Contact>(request);
            ResponseApi<Contact?> response = await contactRepository.CreateAsync(contact);

            if(response.Data is null) return new(null, 400, "Falha ao criar Contato.");
            return new(response.Data, 201, "Contato criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Contact?>> UpdateAsync(UpdateContactDTO request)
    {
        try
        {
            ResponseApi<Contact?> contactResponse = await contactRepository.GetByIdAsync(request.Id);
            if(contactResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Contact contact = _mapper.Map<Contact>(request);
            contact.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Contact?> response = await contactRepository.UpdateAsync(contact);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Contact>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Contact> contact = await contactRepository.DeleteAsync(id);
            if(!contact.IsSuccess) return new(null, 400, contact.Message);
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