using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class AddressService(IAddressRepository addressRepository, IMapper _mapper) : IAddressService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Address> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> addresss = await addressRepository.GetAllAsync(pagination);
            int count = await addressRepository.GetCountDocumentsAsync(pagination);
            return new(addresss.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> address = await addressRepository.GetByIdAggregateAsync(id);
            if(address.Data is null) return new(null, 404, "Item não encontrado");
            return new(address.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Address?>> GetByParentIdAsync(string parentId, string parent)
    {
        try
        {
            ResponseApi<Address?> address = await addressRepository.GetByParentIdAsync(parentId, parent);
            if(address.Data is null) return new(null, 404, "Endereço não encontrado");
            return new(address.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Address?>> CreateAsync(CreateAddressDTO request)
    {
        try
        {
            Address address = _mapper.Map<Address>(request);
            ResponseApi<Address?> response = await addressRepository.CreateAsync(address);

            if(response.Data is null) return new(null, 400, "Falha ao salvar Endereço.");
            return new(response.Data, 201, "Endereço criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Address?>> UpdateAsync(UpdateAddressDTO request)
    {
        try
        {
            ResponseApi<Address?> addressResponse = await addressRepository.GetByIdAsync(request.Id);
            if(addressResponse.Data is null) return new(null, 404, "Falha ao atualizar Endereço");
            
            Address address = _mapper.Map<Address>(request);
            address.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Address?> response = await addressRepository.UpdateAsync(address);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar Endereço");
            return new(response.Data, 201, "Endereço atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Address>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Address> address = await addressRepository.DeleteAsync(id);
            if(!address.IsSuccess) return new(null, 400, address.Message);
            return new(null, 204, "Endereço Excluído com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion 
}
}