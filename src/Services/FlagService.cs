using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class FlagService(IFlagRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IFlagService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Flag> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Flags = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Flags.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Flag = await repository.GetByIdAggregateAsync(id);
            if(Flag.Data is null) return new(null, 404, "Loja não encontrada");
            return new(Flag.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Flag?>> CreateAsync(CreateFlagDTO request)
    {
        try
        {
            Flag Flag = _mapper.Map<Flag>(request);
            ResponseApi<Flag?> response = await repository.CreateAsync(Flag);

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
    public async Task<ResponseApi<Flag?>> UpdateAsync(UpdateFlagDTO request)
    {
        try
        {
            ResponseApi<Flag?> FlagResponse = await repository.GetByIdAsync(request.Id);
            if(FlagResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Flag Flag = _mapper.Map<Flag>(request);
            Flag.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Flag?> response = await repository.UpdateAsync(Flag);
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
    public async Task<ResponseApi<Flag>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Flag> Flag = await repository.DeleteAsync(id);
            if(!Flag.IsSuccess) return new(null, 400, Flag.Message);
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