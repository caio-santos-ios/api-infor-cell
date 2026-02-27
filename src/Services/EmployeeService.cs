using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Templates;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class EmployeeService(IEmployeeRepository repository, IUserRepository userRepository, IMapper _mapper) : IEmployeeService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Employee> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> users = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(users.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<dynamic?>> GetLoggedAsync(string id)
    {
        try
        {
            ResponseApi<dynamic?> user = await repository.GetLoggedAsync(id);
            if(user.Data is null) return new(null, 404, "Usuário não encontrado");
            return new(user.Data);
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
            ResponseApi<dynamic?> User = await repository.GetByIdAggregateAsync(id);
            if(User.Data is null) return new(null, 404, "Profissional não encontrada");
            return new(User.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<User>>> GetSellersAsync(string planId, string companyId, string storeId)
    {
        try
        {
            ResponseApi<List<User>> employee = await repository.GetSellersAsync(planId, companyId, storeId);
            return new(employee.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<User>>> GetTechniciansAsync(string planId, string companyId, string storeId)
    {
        try
        {
            ResponseApi<List<User>> employee = await repository.GetTechniciansAsync(planId, companyId, storeId);
            return new(employee.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO request)
    {
        try
        {
            ResponseApi<User?> userResponse = await userRepository.GetByIdAsync(request.Id);
            if(userResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ResponseApi<Employee?> employeeResponse = await repository.GetByUserIdAsync(request.Id);
            if(employeeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            
            // ResponseApi<User?> cpfExisted = await repository.GetByCpfAsync(request.CPF, request.Id);
            // if(cpfExisted.Data is not null) return new(null, 404, "CPF já cadastrado");

            // ResponseApi<User?> emailExisted = await repository.GetByEmailAsync(request.Email, request.Id);
            // if(emailExisted.Data is not null) return new(null, 404, "E-mail já cadastrado");
            
            Employee employee = _mapper.Map<Employee>(request);
            employee.UpdatedAt = DateTime.UtcNow;
            employee.CreatedAt = employeeResponse.Data.CreatedAt;
            employee.Calendar = employeeResponse.Data.Calendar;

            ResponseApi<Employee?> response = await repository.UpdateAsync(employee);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

            return new(userResponse.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<User?>> UpdateModuleAsync(UpdateModuleEmployeeDTO request)
    {
        try
        {
            // ResponseApi<User?> user = await repository.GetByIdAsync(request.Id);
            // if(user.Data is null) return new(null, 404, "Falha ao atualizar");

            ResponseApi<User?> userResponse = await userRepository.GetByIdAsync(request.Id);
            if(userResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            // ResponseApi<Employee?> employeeResponse = await repository.GetByUserIdAsync(request.Id);
            // if(employeeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            userResponse.Data.UpdatedAt = DateTime.UtcNow;
            List<api_infor_cell.src.Models.Module> modules = [];
            foreach (var module in request.Modules)
            {
                List<api_infor_cell.src.Models.Routine> routines = [];

                foreach (var routine in module.Routines)
                {
                    routines.Add(new () 
                    {
                        Code = routine.Code,
                        Description = routine.Description,
                        Permissions = new ()
                        {
                            Create = routine.Permissions.Create,
                            Read = routine.Permissions.Read,
                            Update = routine.Permissions.Update,
                            Delete = routine.Permissions.Delete
                        }
                    });
                }
                
                modules.Add(new () 
                {
                    Code = module.Code,
                    Description = module.Description,
                    Routines = routines
                });
            };

            userResponse.Data.Modules = modules;

            ResponseApi<User?> response = await userRepository.UpdateAsync(userResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<User?>> UpdateCalendarAsync(UpdateCalendarEmployeeDTO request)
    {
        try
        {
            // ResponseApi<User?> user = await repository.GetByIdAsync(request.Id);
            // if(user.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
            if(user.Data is null) return new(null, 404, "Falha ao atualizar");

            ResponseApi<Employee?> userResponse = await repository.GetByUserIdAsync(user.Data.Id);
            if(userResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            userResponse.Data.UpdatedAt = DateTime.UtcNow;
            userResponse.Data.Calendar = request.Calendar;

            ResponseApi<Employee?> response = await repository.UpdateAsync(userResponse.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(user.Data, 200, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region DELETE
    public async Task<ResponseApi<User>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<User> User = await userRepository.DeleteAsync(id);
            if(!User.IsSuccess) return new(null, 400, User.Message);
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