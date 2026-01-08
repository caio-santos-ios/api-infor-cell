using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class EmployeeService(IEmployeeRepository repository, CloudinaryHandler cloudinaryHandler, IMapper _mapper) : IEmployeeService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Employee> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Employees = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Employees.Data, count, pagination.PageNumber, pagination.PageSize);
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
            ResponseApi<dynamic?> Employee = await repository.GetByIdAggregateAsync(id);
            if(Employee.Data is null) return new(null, 404, "Loja não encontrada");
            return new(Employee.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Employee?>> CreateAsync(CreateEmployeeDTO request)
    {
        try
        {
            Employee Employee = _mapper.Map<Employee>(request);
            ResponseApi<Employee?> response = await repository.CreateAsync(Employee);

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
    public async Task<ResponseApi<Employee?>> UpdateAsync(UpdateEmployeeDTO request)
    {
        try
        {
            ResponseApi<Employee?> EmployeeResponse = await repository.GetByIdAsync(request.Id);
            if(EmployeeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            Employee Employee = _mapper.Map<Employee>(request);
            Employee.UpdatedAt = DateTime.UtcNow;

            ResponseApi<Employee?> response = await repository.UpdateAsync(Employee);
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
    public async Task<ResponseApi<Employee>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Employee> Employee = await repository.DeleteAsync(id);
            if(!Employee.IsSuccess) return new(null, 400, Employee.Message);
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