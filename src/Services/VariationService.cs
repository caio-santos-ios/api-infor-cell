using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class VariationService(IVariationRepository repository, IMapper _mapper) : IVariationService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Variation> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Variations = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Variations.Data, count, pagination.PageNumber, pagination.PageSize);
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
            PaginationUtil<Variation> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> variations = await repository.GetSelectAsync(pagination);
            return new(variations.Data);
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
            ResponseApi<dynamic?> Variation = await repository.GetByIdAggregateAsync(id);
            if(Variation.Data is null) return new(null, 404, "Variação não encontrado");
            return new(Variation.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Variation?>> CreateAsync(CreateVariationDTO request)
    {
        try
        {
            Variation variation = _mapper.Map<Variation>(request);
            ResponseApi<long> code = await repository.GetNextCodeAsync(request.Company, request.Store);
            variation.Code = code.Data.ToString().PadLeft(6, '0');
            ResponseApi<Variation?> response = await repository.CreateAsync(variation);

            if(response.Data is null) return new(null, 400, "Falha ao criar Variação.");
            return new(response.Data, 201, "Variação criado com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Variation?>> UpdateAsync(UpdateVariationDTO request)
    {
        try
        {
            ResponseApi<Variation?> variationResponse = await repository.GetByIdAsync(request.Id);
            if(variationResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            if(request.SerialAction == "create" || request.SerialAction == "update")
            {
                ResponseApi<List<Variation>> existed = await repository.GetSerialExistedAsync(request.Serial);
                if(existed.Data is null || existed.Data.Count > 1) return new(null, 400, "Serial já cadastrado");

                var variati = existed.Data.Where(x => x.Id != request.Id && x.Items.Where(i => i.Serial.Where(s => s.Code == request.Serial).Any()).Any()).FirstOrDefault();
                if(variati is not null) return new(null, 400, "Serial já cadastrado");

                var existedSerial = request.Items.Where(i => i.Serial.Where(s => s.Code == request.Serial).Count() > 1).Count();

                if(existedSerial >= 1) return new(null, 400, "Serial já cadastrado");    
            };
            
            Variation variation = _mapper.Map<Variation>(request);
            variation.UpdatedAt = DateTime.UtcNow;
            variation.CreatedAt = variationResponse.Data.CreatedAt;
            variation.Code = variationResponse.Data.Code;

            ResponseApi<Variation?> response = await repository.UpdateAsync(variation);
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
    public async Task<ResponseApi<Variation>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Variation> Variation = await repository.DeleteAsync(id);
            if(!Variation.IsSuccess) return new(null, 400, Variation.Message);
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