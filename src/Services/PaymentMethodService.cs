using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class PaymentMethodService(IPaymentMethodRepository repository, IMapper _mapper) : IPaymentMethodService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<PaymentMethod> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> PaymentMethods = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(PaymentMethods.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> PaymentMethod = await repository.GetByIdAggregateAsync(id);
            if(PaymentMethod.Data is null) return new(null, 404, "Loja não encontrada");
            return new(PaymentMethod.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<PaymentMethod?>> CreateAsync(CreatePaymentMethodDTO request)
    {
        try
        {
            PaymentMethod PaymentMethod = _mapper.Map<PaymentMethod>(request);
            ResponseApi<PaymentMethod?> response = await repository.CreateAsync(PaymentMethod);

            if(response.Data is null) return new(null, 400, "Falha ao criar Loja.");
            return new(response.Data, 201, "Loja criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<PaymentMethod?>> UpdateAsync(UpdatePaymentMethodDTO request)
    {
        try
        {
            ResponseApi<PaymentMethod?> PaymentMethodResponse = await repository.GetByIdAsync(request.Id);
            if(PaymentMethodResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            PaymentMethod PaymentMethod = _mapper.Map<PaymentMethod>(request);
            PaymentMethod.UpdatedAt = DateTime.UtcNow;

            ResponseApi<PaymentMethod?> response = await repository.UpdateAsync(PaymentMethod);
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
    public async Task<ResponseApi<PaymentMethod>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<PaymentMethod> PaymentMethod = await repository.DeleteAsync(id);
            if(!PaymentMethod.IsSuccess) return new(null, 400, PaymentMethod.Message);
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