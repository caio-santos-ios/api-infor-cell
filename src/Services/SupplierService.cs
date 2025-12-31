using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class SupplierService(ISupplierRepository supplier, IAddressRepository addressRepository, IMapper _mapper) : ISupplierService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Supplier> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> accrediteds = await supplier.GetAllAsync(pagination);
            int count = await supplier.GetCountDocumentsAsync(pagination);
            return new(accrediteds.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> accredited = await supplier.GetByIdAggregateAsync(id);
            if(accredited.Data is null) return new(null, 404, "Fornecedor não encontrado");
            return new(accredited.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }

    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Supplier> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> accrediteds = await supplier.GetSelectAsync(pagination);
            return new(accrediteds.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Supplier?>> CreateAsync(CreateSupplierDTO request)
    {
        try
        {
            Supplier accredited = _mapper.Map<Supplier>(request);
            ResponseApi<Supplier?> response = await supplier.CreateAsync(accredited);

            if(response.Data is null) return new(null, 400, "Falha ao criar Fornecedor.");

            if(!string.IsNullOrEmpty(request.Address.Id))
            {
                ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(request.Address.ParentId, request.Address.Parent);
                if(!findAddress.IsSuccess || findAddress.Data is null) return new(null, 400, "Falha ao atualizar.");
                
                request.Address.Id = findAddress.Data.Id;
            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Address);
                address.ParentId = response.Data.Id;
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Fornecedor.");
            };

            return new(response.Data, 201, "Fornecedor criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Supplier?>> UpdateAsync(UpdateSupplierDTO request)
    {
        try
        {
            ResponseApi<Supplier?> accreditedResponse = await supplier.GetByIdAsync(request.Id);
            if(accreditedResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Supplier accredited = _mapper.Map<Supplier>(request);
            accredited.UpdatedAt = DateTime.UtcNow;
            accredited.CreatedAt = accreditedResponse.Data.CreatedAt;

            ResponseApi<Supplier?> response = await supplier.UpdateAsync(accredited);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            if(!string.IsNullOrEmpty(request.Address.Id))
            {
                ResponseApi<Address?> findAddress = await addressRepository.GetByParentIdAsync(request.Address.ParentId, request.Address.Parent);
                if(!findAddress.IsSuccess || findAddress.Data is null) return new(null, 400, "Falha ao atualizar.");
                
                request.Address.Id = findAddress.Data.Id;
            
                ResponseApi<Address?> addressResponse = await addressRepository.UpdateAsync(request.Address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao atualizar.");
            }
            else
            {
                Address address = _mapper.Map<Address>(request.Address);
                ResponseApi<Address?> addressResponse = await addressRepository.CreateAsync(address);
                if(!addressResponse.IsSuccess) return new(null, 400, "Falha ao criar Item.");
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
    public async Task<ResponseApi<Supplier>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Supplier> accredited = await supplier.DeleteAsync(id);
            if(!accredited.IsSuccess) return new(null, 400, accredited.Message);
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