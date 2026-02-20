using api_infor_cell.src.Handlers;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class ServiceOrderService(IServiceOrderRepository repository, IMapper _mapper) : IServiceOrderService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ServiceOrder> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> serviceOrders = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(serviceOrders.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> serviceOrder = await repository.GetByIdAggregateAsync(id);
                if (serviceOrder.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");
                return new(serviceOrder.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei)
        {
            try
            {
                ResponseApi<dynamic?> result = await repository.CheckWarrantyAsync(customerId, serialImei);
                return new(result.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<ServiceOrder?>> CreateAsync(CreateServiceOrderDTO request)
        {
            try
            {
                ResponseApi<long> code = await repository.GetNextCodeAsync(request.Plan, request.Company, request.Store);

                ServiceOrder serviceOrder = _mapper.Map<ServiceOrder>(request);
                serviceOrder.Status = "open";
                serviceOrder.OpenedAt = DateTime.UtcNow;
                serviceOrder.OpenedByUserId = request.CreatedBy;
                serviceOrder.Code = code.Data.ToString().PadLeft(6, '0');
                serviceOrder.Device = new()
                {
                    Type = request.DeviceType,
                    BrandId = request.BrandId,
                    // BrandName = request.BrandName,
                    // ModelId = request.ModelId,
                    // ModelName = request.ModelName,
                    Color = request.Color,
                    SerialImei = request.SerialImei,
                    CustomerReportedIssue = request.CustomerReportedIssue,
                    UnlockPassword = request.UnlockPassword,
                    Accessories = request.Accessories,
                    PhysicalCondition = request.PhysicalCondition,
                };

                // ServiceOrder serviceOrder = new()
                // {
                //     OpenedByUserId = request.OpenedByUserId,
                //     OpenedAt = DateTime.UtcNow,
                //     CustomerId = request.CustomerId,
                //     Status = "open",
                //     Notes = request.Notes,
                //     Company = request.Company,
                //     Store = request.Store,
                //     Plan = request.Plan,
                //     CreatedBy = request.CreatedBy,
                //     Device = new ServiceOrderDevice
                //     {
                //         Type = request.DeviceType,
                //         BrandId = request.BrandId,
                //         BrandName = request.BrandName,
                //         ModelId = request.ModelId,
                //         ModelName = request.ModelName,
                //         Color = request.Color,
                //         SerialImei = request.SerialImei,
                //         CustomerReportedIssue = request.CustomerReportedIssue,
                //         UnlockPassword = request.UnlockPassword,
                //         Accessories = request.Accessories,
                //         PhysicalCondition = request.PhysicalCondition,
                //     }
                // };

                // Check for warranty
                if (!string.IsNullOrEmpty(request.SerialImei) || !string.IsNullOrEmpty(request.CustomerId))
                {
                    ResponseApi<dynamic?> warrantyCheck = await repository.CheckWarrantyAsync(request.CustomerId, request.SerialImei);
                    if (warrantyCheck.Data != null)
                    {
                        serviceOrder.IsWarrantyInternal = true;
                    }
                }

                ResponseApi<ServiceOrder?> response = await repository.CreateAsync(serviceOrder);
                if (response.Data is null) return new(null, 400, "Falha ao criar Ordem de Serviço.");
                return new(response.Data, 201, "Ordem de Serviço criada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<ServiceOrder?>> UpdateAsync(UpdateServiceOrderDTO request)
        {
            try
            {
                ResponseApi<ServiceOrder?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");

                ServiceOrder serviceOrder = existing.Data;
                serviceOrder.CustomerId = request.CustomerId;
                serviceOrder.Status = string.IsNullOrEmpty(request.Status) ? serviceOrder.Status : request.Status;
                serviceOrder.Notes = request.Notes;
                serviceOrder.CancelReason = request.CancelReason;
                serviceOrder.DiscountValue = request.DiscountValue;
                serviceOrder.DiscountType = request.DiscountType;
                serviceOrder.UpdatedAt = DateTime.UtcNow;
                serviceOrder.UpdatedBy = request.UpdatedBy;

                serviceOrder.Device.Type = request.DeviceType;
                serviceOrder.Device.BrandId = request.BrandId;
                // serviceOrder.Device.BrandName = request.BrandName;
                // serviceOrder.Device.ModelId = request.ModelId;
                serviceOrder.Device.ModelName = request.ModelName;
                serviceOrder.Device.Color = request.Color;
                serviceOrder.Device.SerialImei = request.SerialImei;
                serviceOrder.Device.CustomerReportedIssue = request.CustomerReportedIssue;
                serviceOrder.Device.UnlockPassword = request.UnlockPassword;
                serviceOrder.Device.Accessories = request.Accessories;
                serviceOrder.Device.PhysicalCondition = request.PhysicalCondition;

                serviceOrder.Laudo.TechnicalReport = request.TechnicalReport;
                serviceOrder.Laudo.TestsPerformed = request.TestsPerformed;
                serviceOrder.Laudo.RepairStatus = request.RepairStatus;

                ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(serviceOrder);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Ordem de Serviço.");
                return new(response.Data, 200, "Ordem de Serviço atualizada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<ServiceOrder?>> CloseAsync(CloseServiceOrderDTO request)
        {
            try
            {
                ResponseApi<ServiceOrder?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");

                ServiceOrder serviceOrder = existing.Data;
                serviceOrder.Status = "closed";
                serviceOrder.ClosedAt = DateTime.UtcNow;
                serviceOrder.ClosedByUserId = request.ClosedByUserId;
                serviceOrder.WarrantyDays = request.WarrantyDays;
                serviceOrder.WarrantyUntil = request.WarrantyUntil ?? DateTime.UtcNow.AddDays(request.WarrantyDays);
                serviceOrder.UpdatedAt = DateTime.UtcNow;
                serviceOrder.UpdatedBy = request.UpdatedBy;

                if (!serviceOrder.IsWarrantyInternal && !string.IsNullOrEmpty(request.PaymentMethodId))
                {
                    serviceOrder.Payment = new ServiceOrderPayment
                    {
                        PaymentMethodId = request.PaymentMethodId,
                        // PaymentMethodName = request.PaymentMethodName,
                        NumberOfInstallments = request.NumberOfInstallments,
                        PaidAt = DateTime.UtcNow,
                        PaidByUserId = request.ClosedByUserId
                    };
                }

                ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(serviceOrder);
                if (!response.IsSuccess) return new(null, 400, "Falha ao encerrar Ordem de Serviço.");
                return new(response.Data, 200, "Ordem de Serviço encerrada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<ServiceOrder>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<ServiceOrder> serviceOrder = await repository.DeleteAsync(id);
                if (!serviceOrder.IsSuccess) return new(null, 400, serviceOrder.Message);
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