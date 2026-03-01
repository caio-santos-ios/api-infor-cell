using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
using AutoMapper;

namespace api_infor_cell.src.Services
{
    public class FiscalDocumentService(
        IFiscalDocumentRepository repository,
        ISalesOrderRepository salesOrderRepository,
        ISalesOrderItemRepository salesOrderItemRepository,
        IServiceOrderRepository serviceOrderRepository,
        IServiceOrderItemRepository serviceOrderItemRepository,
        ICustomerRepository customerRepository,
        IStoreRepository storeRepository,
        IProductRepository productRepository,
        ISefazProvider sefazProvider,
        ILogger<FiscalDocumentService> logger) : IFiscalDocumentService
    {
        // ─── EMIT ─────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<FiscalDocument?>> EmitAsync(EmitFiscalDocumentDTO request, string userId)
        {
            try
            {
                // 1. Busca configuração fiscal da loja
                ResponseApi<FiscalConfig?> configResp = await repository.GetConfigByStoreAsync(request.Store);
                if (configResp.Data is null)
                    return new(null, 422, "Configuração fiscal não encontrada. Configure o módulo fiscal antes de emitir.");

                FiscalConfig config = configResp.Data;

                if (string.IsNullOrWhiteSpace(config.CertificateBase64))
                    return new(null, 422, "Certificado digital não configurado.");

                // 2. Evita emissão duplicada (idempotência)
                ResponseApi<FiscalDocument?> existing = await repository.GetByOriginAsync(request.OriginId, request.OriginType);
                if (existing.Data is not null && existing.Data.Status is "Authorized" or "Processing")
                    return new(existing.Data, 409, $"Já existe um documento fiscal {existing.Data.Status} para esta origem.");

                // 3. Monta o documento fiscal a partir da venda ou OS
                ResponseApi<FiscalDocument?> buildResp = request.OriginType == "sales_order"
                    ? await BuildFromSalesOrderAsync(request, config, userId)
                    : await BuildFromServiceOrderAsync(request, config, userId);

                if (!buildResp.IsSuccess || buildResp.Data is null)
                    return buildResp;

                FiscalDocument doc = buildResp.Data;

                // 4. Persiste como Pending
                doc.Status = "Pending";
                doc.CreatedBy = userId;
                doc.UpdatedBy = userId;
                await repository.CreateAsync(doc);

                // 5. Processa de forma assíncrona (não bloqueia o retorno)
                _ = ProcessFiscalDocumentAsync(doc, config, userId);

                return new(doc, 202, "Documento fiscal enfileirado para emissão. Acompanhe o status.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao emitir documento fiscal origem {OriginId}", request.OriginId);
                return new(null, 500, "Erro inesperado ao emitir documento fiscal.");
            }
        }

        // ─── PROCESS (assíncrono: Build → Sign → Transmit → Persist) ─────────────
        private async Task ProcessFiscalDocumentAsync(FiscalDocument doc, FiscalConfig config, string userId)
        {
            try
            {
                // Status: Processing
                doc.Status = "Processing";
                doc.IssuedAt = DateTime.UtcNow;
                await repository.UpdateAsync(doc);

                // Gera número único (atômico no MongoDB)
                doc.Number = await repository.GetNextNumberAsync(doc.Store, doc.Model);

                // Build + Sign
                SefazBuildResult buildResult = await sefazProvider.BuildAndSignAsync(doc, config);
                if (!buildResult.Success)
                {
                    await SetStatusAsync(doc, "Rejected", buildResult.ErrorMessage, userId);
                    return;
                }

                doc.AccessKey = buildResult.AccessKey;
                doc.XmlSent = buildResult.SignedXml;
                await repository.UpdateAsync(doc);

                // Transmit com retentativa
                SefazTransmitResult transmitResult = await TransmitWithRetryAsync(buildResult.SignedXml, doc, config, maxRetries: 3);

                if (transmitResult.Authorized)
                {
                    doc.Status = "Authorized";
                    doc.Protocol = transmitResult.Protocol;
                    doc.XmlAuthorized = transmitResult.AuthorizedXml;
                    doc.AuthorizedAt = DateTime.UtcNow;
                    doc.SefazReturnCode = transmitResult.ReturnCode;
                    doc.SefazReturnMessage = transmitResult.ReturnMessage;
                    doc.UpdatedBy = userId;
                    await repository.UpdateAsync(doc);
                    logger.LogInformation("NF {Number} autorizada. Protocolo: {Protocol}", doc.Number, doc.Protocol);
                }
                else if (transmitResult.Denied)
                {
                    await SetStatusAsync(doc, "Denied", $"{transmitResult.ReturnCode} - {transmitResult.ReturnMessage}", userId);
                }
                else if (transmitResult.CommunicationError)
                {
                    // Contingência: salva para reprocessar depois
                    doc.IsContingency = true;
                    doc.ContingencyReason = transmitResult.ReturnMessage;
                    doc.ContingencyStartedAt = DateTime.UtcNow;
                    await SetStatusAsync(doc, "Contingency", transmitResult.ReturnMessage, userId);
                }
                else
                {
                    await SetStatusAsync(doc, "Rejected", $"{transmitResult.ReturnCode} - {transmitResult.ReturnMessage}", userId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro no processamento fiscal doc {Id}", doc.Id);
                await SetStatusAsync(doc, "Rejected", ex.Message, userId);
            }
        }

        private async Task<SefazTransmitResult> TransmitWithRetryAsync(string signedXml, FiscalDocument doc, FiscalConfig config, int maxRetries)
        {
            SefazTransmitResult result = new();
            for (int i = 1; i <= maxRetries; i++)
            {
                result = await sefazProvider.TransmitAsync(signedXml, doc, config);
                doc.Attempts.Add(new()
                {
                    ReturnCode = result.ReturnCode,
                    ReturnMessage = result.ReturnMessage,
                });
                await repository.UpdateAsync(doc);

                if (result.Authorized || result.Denied) break;
                if (result.Rejected) break; // Rejeição não melhora com retentativa

                if (i < maxRetries)
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
            }
            return result;
        }

        private async Task SetStatusAsync(FiscalDocument doc, string status, string message, string userId)
        {
            doc.Status = status;
            doc.SefazReturnMessage = message;
            doc.UpdatedBy = userId;
            doc.UpdatedAt = DateTime.UtcNow;
            await repository.UpdateAsync(doc);
        }

        // ─── BUILD FROM SALES ORDER ───────────────────────────────────────────────
        private async Task<ResponseApi<FiscalDocument?>> BuildFromSalesOrderAsync(EmitFiscalDocumentDTO request, FiscalConfig config, string userId)
        {
            // Busca pedido de venda
            ResponseApi<dynamic?> orderResp = await salesOrderRepository.GetByIdAggregateAsync(request.OriginId);
            if (orderResp.Data is null) return new(null, 404, "Pedido de venda não encontrado.");

            ResponseApi<SalesOrder?> orderRes = await salesOrderRepository.GetByIdAsync(request.OriginId);
            if (orderRes.Data is null) return new(null, 404, "Pedido de venda não encontrado.");
            SalesOrder order = orderRes.Data;

            // Busca itens
            ResponseApi<List<SalesOrderItem>> itemsRes = await salesOrderItemRepository.GetBySalesOrderIdAsync(request.OriginId, request.Plan, request.Company, request.Store);
            if (itemsRes.Data is null) return new(null, 422, "Pedido sem itens.");

            List<SalesOrderItem> items = itemsRes.Data;
            if (items.Count == 0) return new(null, 422, "Pedido sem itens.");

            // Busca cliente e loja
            ResponseApi<Customer?> customerRes = await customerRepository.GetByIdAsync(order.CustomerId);
            Customer? customer = customerRes.Data is not null ? customerRes.Data : null;
            
            ResponseApi<Store?> storeRes = await storeRepository.GetByIdAsync(order.Store);
            if (storeRes.Data is null) return new(null, 422, "Loja não encontrada.");
            Store store = storeRes.Data;

            FiscalDocument doc = new()
            {
                OriginId = request.OriginId,
                OriginType = "sales_order",
                Model = request.Model,
                Series = request.Model == 55 ? config.SeriesNfe : config.SeriesNfce,
                Environment = config.Environment,
                Company = order.Company,
                Store = order.Store,
                Plan = order.Plan,
                PaymentMethodId = order.Payment.PaymentMethodId,
                AdditionalInfo = $"Pedido de Venda #{order.Code}",
                Issuer = BuildIssuer(store, config),
                Recipient = BuildRecipient(customer),
            };

            // Itens
            int itemNum = 1;
            foreach (SalesOrderItem item in items)
            {
                ResponseApi<Product?> productRes = await productRepository.GetByIdAsync(item.ProductId);
                doc.Items.Add(BuildFiscalItem(itemNum++, item, productRes.Data, config, order.Store, customer));
            }

            doc.Totals = CalculateTotals(doc.Items, order.Payment.Freight, order.Payment.DiscountValue);
            return new(doc);
        }

        // ─── BUILD FROM SERVICE ORDER ─────────────────────────────────────────────
        private async Task<ResponseApi<FiscalDocument?>> BuildFromServiceOrderAsync(EmitFiscalDocumentDTO request, FiscalConfig config, string userId)
        {
            ResponseApi<ServiceOrder?> order = await serviceOrderRepository.GetByIdAsync(request.OriginId);
            if (order.Data is null) return new(null, 404, "Ordem de serviço não encontrada.");
            if (order.Data.Status != "closed") return new(null, 422, "A O.S. precisa estar fechada para emitir nota fiscal.");

            ResponseApi<List<ServiceOrderItem>> items = await serviceOrderItemRepository.GetByServiceOrderIdAsync(request.OriginId);
            if (items.Data is null) return new(null, 422, "OS sem itens.");
            if (items.Data.Count == 0) return new(null, 422, "OS sem itens.");

            ResponseApi<Customer?> customer = await customerRepository.GetByIdAsync(order.Data.CustomerId);
            ResponseApi<Store?> store = await storeRepository.GetByIdAsync(order.Data.Store);
            if (store.Data is null) return new(null, 422, "Loja não encontrada.");

            FiscalDocument doc = new()
            {
                OriginId = request.OriginId,
                OriginType = "service_order",
                Model = request.Model,
                Series = request.Model == 55 ? config.SeriesNfe : config.SeriesNfce,
                Environment = config.Environment,
                Company = order.Data.Company,
                Store = order.Data.Store,
                Plan = order.Data.Plan,
                PaymentMethodId = order.Data.Payment.PaymentMethodId,
                AdditionalInfo = $"OS #{order.Data.Code} - IMEI/Serial: {order.Data.Device.SerialImei}",
                Issuer = BuildIssuer(store.Data, config),
                Recipient = BuildRecipient(customer.Data),
            };

            // Na OS: produtos (peças) e serviços têm CFOPs diferentes
            int itemNum = 1;
            foreach (ServiceOrderItem item in items.Data)
            {
                ResponseApi<Product?> product = item.ProductId != string.Empty ? await productRepository.GetByIdAsync(item.ProductId) : null!;

                doc.Items.Add(BuildFiscalItemFromOS(itemNum++, item, product!.Data, config, customer.Data));
            }

            doc.Totals = CalculateTotals(doc.Items, 0, order.Data.DiscountValue);
            return new(doc);
        }

        // ─── BUILDERS ────────────────────────────────────────────────────────────
        private FiscalIssuer BuildIssuer(Store store, FiscalConfig config) => new()
        {
            Cnpj = store.Document.Replace(".", "").Replace("/", "").Replace("-", ""),
            CorporateName = store.CorporateName,
            TradeName = store.TradeName,
            StateRegistration = store.StateRegistration,
            MunicipalRegistration = store.MunicipalRegistration,
            TaxRegime = config.TaxRegime,
            Street = config.Street,
            Number = config.AddressNumber,
            District = config.District,
            City = config.City,
            CityCode = config.CityCode,
            State = config.State,
            ZipCode = config.ZipCode,
            Phone = store.Phone,
        };

        private FiscalRecipient BuildRecipient(Customer? customer) => new()
        {
            Document = customer?.Document.Replace(".", "").Replace("/", "").Replace("-", "") ?? "",
            Name = customer?.TradeName ?? customer?.CorporateName ?? "CONSUMIDOR FINAL",
            Email = customer?.Email ?? "",
            Phone = customer?.Phone ?? "",
            IeIndicator = 9, // Não contribuinte (padrão para PF/consumidor)
        };

        private FiscalDocumentItem BuildFiscalItem(int num, SalesOrderItem item, Product? product, FiscalConfig config, string storeState, Customer? customer) => new()
        {
            ItemNumber = num,
            ProductId = item.ProductId,
            Description = product?.Name ?? "Produto",
            Ncm = product?.Ncm ?? "84713012",
            Cest = product?.Cest.ToString() ?? "",
            Cfop = config.DefaultCfopInState, // Ajustar conforme UF origem/destino
            UnitOfMeasure = product?.UnitOfMeasure ?? "UN",
            Quantity = item.Quantity,
            UnitPrice = item.Value,
            Discount = item.DiscountValue,
            Total = item.Total,
            Csosn = "400",
            IcmsOrigin = product?.Origin ?? "0",
            PisCst = "07",
            CofinsCst = "07",
            ItemType = "product",
            Serial = item.Serial,
        };

        private FiscalDocumentItem BuildFiscalItemFromOS(int num, ServiceOrderItem item, Product? product, FiscalConfig config, Customer? customer) => new()
        {
            ItemNumber = num,
            ProductId = item.ProductId,
            Description = item.Description,
            // Peça (part): usa NCM do produto / Serviço (service): NCM de serviço de reparo
            Ncm = item.ItemType == "part" ? (product?.Ncm ?? "84713012") : "85171200",
            Cfop = item.ItemType == "service" ? config.DefaultCfopService : config.DefaultCfopInState,
            UnitOfMeasure = "UN",
            Quantity = item.Quantity,
            UnitPrice = item.Price,
            Discount = 0,
            Total = item.Total,
            Csosn = "400",
            IcmsOrigin = "0",
            PisCst = "07",
            CofinsCst = "07",
            ItemType = item.ItemType,
        };

        private FiscalTotals CalculateTotals(List<FiscalDocumentItem> items, decimal freight, decimal discount) => new()
        {
            ProductsTotal = items.Sum(i => i.Total),
            Discount = discount,
            Freight = freight,
            GrandTotal = items.Sum(i => i.Total) - discount + freight,
            IcmsBase = 0,
            IcmsValue = 0,
            PisValue = items.Sum(i => i.PisValue),
            CofinsValue = items.Sum(i => i.CofinsValue),
        };

        // ─── CANCEL ───────────────────────────────────────────────────────────────
        public async Task<ResponseApi<FiscalDocument?>> CancelAsync(CancelFiscalDocumentDTO request, string userId)
        {
            try
            {
                ResponseApi<FiscalDocument?> docResp = await repository.GetByIdAsync(request.FiscalDocumentId);
                if (docResp.Data is null) return new(null, 404, "Documento fiscal não encontrado.");

                FiscalDocument doc = docResp.Data;
                if (doc.Status != "Authorized") return new(null, 422, "Só é possível cancelar notas autorizadas.");
                if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 15)
                    return new(null, 422, "Motivo do cancelamento deve ter no mínimo 15 caracteres.");

                ResponseApi<FiscalConfig?> configResp = await repository.GetConfigByStoreAsync(doc.Store);
                if (configResp.Data is null) return new(null, 422, "Configuração fiscal não encontrada.");

                SefazEventResult result = await sefazProvider.CancelAsync(doc, request.Reason, configResp.Data);

                FiscalEvent evt = new()
                {
                    FiscalDocumentId = doc.Id,
                    AccessKey = doc.AccessKey,
                    EventType = "Cancellation",
                    Reason = request.Reason,
                    Status = result.Success ? "Authorized" : "Rejected",
                    Protocol = result.Protocol,
                    XmlEvent = result.EventXml,
                    RequestedByUserId = userId,
                    ProcessedAt = DateTime.UtcNow,
                    Company = doc.Company,
                    Store = doc.Store,
                    Plan = doc.Plan,
                };
                await repository.CreateEventAsync(evt);

                if (result.Success)
                {
                    doc.Status = "Cancelled";
                    doc.UpdatedBy = userId;
                    await repository.UpdateAsync(doc);
                    return new(doc, 200, "Nota fiscal cancelada com sucesso.");
                }

                return new(doc, 422, $"Cancelamento rejeitado pela SEFAZ: {result.ReturnCode} - {result.ReturnMessage}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao cancelar documento {Id}", request.FiscalDocumentId);
                return new(null, 500, "Erro inesperado ao cancelar documento fiscal.");
            }
        }

        // ─── CORRECTION LETTER ────────────────────────────────────────────────────
        public async Task<ResponseApi<FiscalEvent?>> SendCorrectionLetterAsync(CorrectionLetterDTO request, string userId)
        {
            try
            {
                ResponseApi<FiscalDocument?> docResp = await repository.GetByIdAsync(request.FiscalDocumentId);
                if (docResp.Data is null) return new(null, 404, "Documento fiscal não encontrado.");

                FiscalDocument doc = docResp.Data;
                if (doc.Status != "Authorized") return new(null, 422, "CC-e só pode ser enviada para notas autorizadas.");
                if (request.CorrectionText.Length < 15)
                    return new(null, 422, "Texto de correção deve ter no mínimo 15 caracteres.");

                ResponseApi<FiscalConfig?> configResp = await repository.GetConfigByStoreAsync(doc.Store);
                if (configResp.Data is null) return new(null, 422, "Configuração fiscal não encontrada.");

                // Sequência: conta quantas CC-e já foram enviadas para esta chave
                int seq = 1; // TODO: buscar do repositório de eventos para incrementar

                SefazEventResult result = await sefazProvider.SendCorrectionLetterAsync(doc, request.CorrectionText, seq, configResp.Data);

                FiscalEvent evt = new()
                {
                    FiscalDocumentId = doc.Id,
                    AccessKey = doc.AccessKey,
                    EventType = "CorrectionLetter",
                    Reason = request.CorrectionText,
                    Status = result.Success ? "Authorized" : "Rejected",
                    Protocol = result.Protocol,
                    XmlEvent = result.EventXml,
                    RequestedByUserId = userId,
                    ProcessedAt = DateTime.UtcNow,
                    Company = doc.Company,
                    Store = doc.Store,
                    Plan = doc.Plan,
                };
                await repository.CreateEventAsync(evt);

                return result.Success
                    ? new(evt, 200, "CC-e enviada com sucesso.")
                    : new(evt, 422, $"CC-e rejeitada: {result.ReturnCode} - {result.ReturnMessage}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao enviar CC-e doc {Id}", request.FiscalDocumentId);
                return new(null, 500, "Erro inesperado ao enviar CC-e.");
            }
        }

        // ─── RETRY (reprocessa Rejected/Contingency) ──────────────────────────────
        public async Task<ResponseApi<FiscalDocument?>> RetryAsync(string fiscalDocumentId, string userId)
        {
            try
            {
                ResponseApi<FiscalDocument?> docResp = await repository.GetByIdAsync(fiscalDocumentId);
                if (docResp.Data is null) return new(null, 404, "Documento não encontrado.");

                FiscalDocument doc = docResp.Data;
                if (doc.Status is not ("Rejected" or "Contingency"))
                    return new(null, 422, "Só é possível reprocessar documentos Rejected ou Contingency.");

                ResponseApi<FiscalConfig?> configResp = await repository.GetConfigByStoreAsync(doc.Store);
                if (configResp.Data is null) return new(null, 422, "Configuração fiscal não encontrada.");

                _ = ProcessFiscalDocumentAsync(doc, configResp.Data, userId);
                return new(doc, 202, "Documento reenfileirado para reprocessamento.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao reprocessar doc {Id}", fiscalDocumentId);
                return new(null, 500, "Erro inesperado ao reprocessar.");
            }
        }

        // ─── GET ──────────────────────────────────────────────────────────────────
        public async Task<ResponseApi<dynamic?>> GetByOriginAsync(string originId, string originType)
        {
            try
            {
                ResponseApi<FiscalDocument?> resp = await repository.GetByOriginAsync(originId, originType);
                return new(resp.Data as dynamic);
            }
            catch { return new(null, 500, "Erro ao buscar documento fiscal."); }
        }

        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<FiscalDocument> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> resp = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(resp.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch { return new(null, 500, "Erro ao buscar documentos fiscais."); }
        }

        // ─── CONFIG ───────────────────────────────────────────────────────────────
        public async Task<ResponseApi<FiscalConfig?>> SaveConfigAsync(SaveFiscalConfigDTO request, string userId)
        {
            try
            {
                ResponseApi<FiscalConfig?> existing = await repository.GetConfigByStoreAsync(request.Store);

                FiscalConfig config = new()
                {
                    Id = existing.Data?.Id ?? string.Empty,
                    Environment = request.Environment,
                    SeriesNfe = request.SeriesNfe,
                    SeriesNfce = request.SeriesNfce,
                    NextNumberNfe = existing.Data?.NextNumberNfe ?? 1,
                    NextNumberNfce = existing.Data?.NextNumberNfce ?? 1,
                    CertificateBase64 = string.IsNullOrEmpty(request.CertificateBase64)
                        ? existing.Data?.CertificateBase64 ?? ""
                        : request.CertificateBase64,
                    CertificatePassword = string.IsNullOrEmpty(request.CertificatePassword)
                        ? existing.Data?.CertificatePassword ?? ""
                        : request.CertificatePassword,
                    Csc = request.Csc,
                    CscId = request.CscId,
                    DefaultCfopInState = request.DefaultCfopInState,
                    DefaultCfopOutState = request.DefaultCfopOutState,
                    DefaultCfopService = request.DefaultCfopService,
                    TaxRegime = request.TaxRegime,
                    Street = request.Street,
                    AddressNumber = request.AddressNumber,
                    District = request.District,
                    City = request.City,
                    CityCode = request.CityCode,
                    State = request.State,
                    ZipCode = request.ZipCode,
                    Company = request.Company,
                    Store = request.Store,
                    Plan = request.Plan,
                    UpdatedBy = userId,
                };

                return await repository.SaveConfigAsync(config);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao salvar configuração fiscal loja {Store}", request.Store);
                return new(null, 500, "Erro ao salvar configuração fiscal.");
            }
        }

        public async Task<ResponseApi<FiscalConfig?>> GetConfigAsync(string storeId) =>
            await repository.GetConfigByStoreAsync(storeId);
    }
}