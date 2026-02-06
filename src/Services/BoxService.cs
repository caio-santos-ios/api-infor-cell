using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class BoxService(IBoxRepository repository, IMapper _mapper) : IBoxService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Box> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> Boxs = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(Boxs.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> Box = await repository.GetByIdAggregateAsync(id);
                if(Box.Data is null) return new(null, 404, "Caixa não encontrada");
                return new(Box.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByCreatedIdAggregateAsync(string createdBy)
        {
            try
            {
                ResponseApi<dynamic?> Box = await repository.GetByCreatedIdAggregateAsync(createdBy);
                return new(Box.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Box?>> CreateAsync(CreateBoxDTO request)
        {
            try
            {
                Box box = _mapper.Map<Box>(request);
                box.Status = "opened";

                ResponseApi<Box?> response = await repository.CreateAsync(box);

                if(response.Data is null) return new(null, 400, "Falha ao abrir Caixa.");
                return new(response.Data, 201, "Caixa aberto com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Box?>> UpdateAsync(UpdateBoxDTO request)
        {
            try
            {
                ResponseApi<Box?> boxResponse = await repository.GetByIdAsync(request.Id);
                if(boxResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Box box = _mapper.Map<Box>(request);
                box.UpdatedAt = DateTime.UtcNow;
                box.CreatedAt = boxResponse.Data.CreatedAt;
                
                ResponseApi<Box?> response = await repository.UpdateAsync(box);
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
        public async Task<ResponseApi<Box>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Box> Box = await repository.DeleteAsync(id);
                if(!Box.IsSuccess) return new(null, 400, Box.Message);
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