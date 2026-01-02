using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class CompanyService(ICompanyRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : ICompanyService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Company> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> companys = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(companys.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> company = await repository.GetByIdAggregateAsync(id);
            if(company.Data is null) return new(null, 404, "Empresa não encontrada");
            return new(company.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Company?>> CreateAsync(CreateCompanyDTO request)
    {
        try
        {
            Company company = _mapper.Map<Company>(request);
            ResponseApi<Company?> response = await repository.CreateAsync(company);

            if(response.Data is null) return new(null, 400, "Falha ao criar Empresa.");
            return new(response.Data, 201, "Empresa criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Company?>> UpdateAsync(UpdateCompanyDTO request)
    {
        try
        {
            ResponseApi<Company?> companyResponse = await repository.GetByIdAsync(request.Id);
            if(companyResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Company company = _mapper.Map<Company>(request);
            company.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Company?> response = await repository.UpdateAsync(company);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Company?>> SavePhotoProfileAsync(SaveCompanyPhotoDTO request)
    {
        try
        {
            if (request.Photo == null || request.Photo.Length == 0) return new(null, 400, "Falha ao salvar foto de perfil");

            ResponseApi<Company?> user = await repository.GetByIdAsync(request.Id);
            if(user.Data is null) return new(null, 404, "Falha ao salvar foto de perfil");

            var tempPath = Path.GetTempFileName();

            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                request.Photo.CopyTo(stream);
            }

            string uriPhoto = await cloudinaryHandler.UploadAttachment("company", request.Photo);
            user.Data.UpdatedAt = DateTime.UtcNow;
            user.Data.Photo = uriPhoto;

            ResponseApi<Company?> response = await repository.UpdateAsync(user.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao salvar logo");
            return new(new () { Photo = response.Data!.Photo }, 201, "Logo salvo com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Company>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Company> company = await repository.DeleteAsync(id);
            if(!company.IsSuccess) return new(null, 400, company.Message);
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