using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class CustomerService(ICustomerRepository repository, IMapper _mapper) : ICustomerService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Customer> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Customers = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Customers.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Customer = await repository.GetByIdAggregateAsync(id);
            if(Customer.Data is null) return new(null, 404, "Cliente não encontrada");
            return new(Customer.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request)
    {
        try
        {
            string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
            string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

            if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
            if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
            if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

            ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, "");
            string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
            if(existedDocument.Data is not null) return new(null, 400, messageExited);

            ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, "");
            if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
            
            Customer Customer = _mapper.Map<Customer>(request);
            if(request.Type == "F")
            {
                Customer.TradeName = request.CorporateName;
            };
            
            ResponseApi<Customer?> response = await repository.CreateAsync(Customer);

            if(response.Data is null) return new(null, 400, "Falha ao criar Cliente.");
            return new(response.Data, 201, "Cliente criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request)
    {
        try
        {
            ResponseApi<Customer?> CustomerResponse = await repository.GetByIdAsync(request.Id);
            if(CustomerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
            string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

            if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
            if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
            if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

            ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, request.Id);
            string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
            if(existedDocument.Data is not null) return new(null, 400, messageExited);

            ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, request.Id);
            if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
            
            Customer Customer = _mapper.Map<Customer>(request);
            Customer.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Customer?> response = await repository.UpdateAsync(Customer);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
   
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Customer>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Customer> Customer = await repository.DeleteAsync(id);
            if(!Customer.IsSuccess) return new(null, 400, Customer.Message);
            return new(null, 204, "Excluída com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}