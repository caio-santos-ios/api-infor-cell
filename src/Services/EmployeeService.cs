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
    public class EmployeeService(IEmployeeRepository repository, IProfilePermissionRepository profilePermissionRepository, MailHandler mailHandler, IMapper _mapper) : IEmployeeService
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
            ResponseApi<dynamic?> Employee = await repository.GetByIdAggregateAsync(id);
            if(Employee.Data is null) return new(null, 404, "Profissional não encontrada");
            return new(Employee.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<Employee>>> GetSellersAsync(string planId, string companyId, string storeId)
    {
        try
        {
            ResponseApi<List<Employee>> employee = await repository.GetSellersAsync(planId, companyId, storeId);
            return new(employee.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<Employee>>> GetTechniciansAsync(string planId, string companyId, string storeId)
    {
        try
        {
            ResponseApi<List<Employee>> employee = await repository.GetTechniciansAsync(planId, companyId, storeId);
            return new(employee.Data);
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
            ResponseApi<Employee?> cpfExisted = await repository.GetByCpfAsync(request.CPF, "");
            if(cpfExisted.Data is not null) return new(null, 404, "CPF já cadastrado");

            ResponseApi<Employee?> emailExisted = await repository.GetByEmailAsync(request.Email, "");
            if(emailExisted.Data is not null) return new(null, 404, "E-mail já cadastrado");

            Employee employee = _mapper.Map<Employee>(request);

            dynamic access = Util.GenerateCodeAccess();

            ResponseApi<ProfilePermission?> profile = await profilePermissionRepository.GetByIdAsync(request.Type);

            employee.ValidatedAccess = true;
            employee.CodeAccessExpiration = null;
            employee.Password = BCrypt.Net.BCrypt.HashPassword(access.CodeAccess);
            employee.Modules = profile.Data is null ? new() : profile.Data.Modules;
            employee.Companies.Add(request.Company);
            employee.Stores.Add(request.Store);

            ResponseApi<Employee?> response = await repository.CreateAsync(employee);
            await mailHandler.SendMailAsync(request.Email, "Primeiro acesso", MailTemplate.FirstAccess(request.Name, request.Email, access.CodeAccess));


            if(response.Data is null) return new(null, 400, "Falha ao criar Profissional.");
            return new(response.Data, 201, "Profissional criada com sucesso.");
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
            ResponseApi<Employee?> employeeResponse = await repository.GetByIdAsync(request.Id);
            if(employeeResponse.Data is null) return new(null, 404, "Falha ao atualizar");
            
            ResponseApi<Employee?> cpfExisted = await repository.GetByCpfAsync(request.CPF, request.Id);
            if(cpfExisted.Data is not null) return new(null, 404, "CPF já cadastrado");

            ResponseApi<Employee?> emailExisted = await repository.GetByEmailAsync(request.Email, request.Id);
            if(emailExisted.Data is not null) return new(null, 404, "E-mail já cadastrado");
            
            Employee employee = _mapper.Map<Employee>(request);
            employee.UpdatedAt = DateTime.UtcNow;
            employee.CreatedAt = employeeResponse.Data.CreatedAt;
            employee.Calendar = employeeResponse.Data.Calendar;

            List<api_infor_cell.src.Models.Module> modules = [];
            if(employee.Type == "technical")
            {
                modules.Add(new ()
                {
                    Code = "D",
                    Description = "Ordens de Serviços",
                    Routines = 
                    [
                        new()
                        {
                            Code = "D1",
                            Description = "Gerenciar O.S.",
                            Permissions = new ()
                            {
                                Create = true,
                                Update = true,
                                Delete = true,
                                Read = true,
                            }
                        },  
                        new()
                        {
                            Code = "D2",
                            Description = "Painel",
                            Permissions = new ()
                            {
                                Create = true,
                                Update = true,
                                Delete = true,
                                Read = true,
                            }
                        },  
                    ]
                });

                employee.Modules = modules;
            }

            if(employee.Type == "seller")
            {
                modules.Add(new ()
                {
                    Code = "C",
                    Description = "Comercial",
                    Routines = 
                    [
                        new()
                        {
                            Code = "C1",
                            Description = "Pedidos de Vendas",
                            Permissions = new ()
                            {
                                Create = true,
                                Update = true,
                                Delete = true,
                                Read = true,
                            }
                        },  
                        new()
                        {
                            Code = "C2",
                            Description = "Orçamentos",
                            Permissions = new ()
                            {
                                Create = true,
                                Update = true,
                                Delete = true,
                                Read = true,
                            }
                        },  
                    ]
                });

                employee.Modules = modules;
            };

            ResponseApi<Employee?> response = await repository.UpdateAsync(employee);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Employee?>> UpdateModuleAsync(UpdateModuleEmployeeDTO request)
    {
        try
        {
            ResponseApi<Employee?> user = await repository.GetByIdAsync(request.Id);
            if(user.Data is null) return new(null, 404, "Falha ao atualizar");
            
            user.Data.UpdatedAt = DateTime.UtcNow;
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

            user.Data.Modules = modules;

            ResponseApi<Employee?> response = await repository.UpdateAsync(user.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizado com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<Employee?>> UpdateCalendarAsync(UpdateCalendarEmployeeDTO request)
    {
        try
        {
            ResponseApi<Employee?> user = await repository.GetByIdAsync(request.Id);
            if(user.Data is null) return new(null, 404, "Falha ao atualizar");
            
            user.Data.UpdatedAt = DateTime.UtcNow;
            user.Data.Calendar = request.Calendar;

            ResponseApi<Employee?> response = await repository.UpdateAsync(user.Data);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 200, "Atualizado com sucesso");
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