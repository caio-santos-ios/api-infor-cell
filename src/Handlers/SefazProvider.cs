using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.NFe;

namespace api_infor_cell.src.Handlers
{
    /// <summary>
    /// Provider SEFAZ via SOAP direto (SP — homologação e produção).
    /// Para trocar de implementação: crie outra classe : ISefazProvider e
    /// altere apenas o registro em Build.cs. O Service não muda.
    /// </summary>
    public class SefazProvider(ILogger<SefazProvider> logger) : ISefazProvider
    {
        // ─── Endpoints SP ────────────────────────────────────────────────────────
        private static readonly Dictionary<string, string> HomologacaoEndpoints = new()
        {
            { "NFeAutorizacao",       "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx" },
            { "NFeRetAutorizacao",    "https://homologacao.nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx" },
            { "NFeConsultaProtocolo", "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx" },
            { "NFeStatusServico",     "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx" },
            { "RecepcaoEvento",       "https://homologacao.nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx" },
        };

        private static readonly Dictionary<string, string> ProducaoEndpoints = new()
        {
            { "NFeAutorizacao",       "https://nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx" },
            { "NFeRetAutorizacao",    "https://nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx" },
            { "NFeConsultaProtocolo", "https://nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx" },
            { "NFeStatusServico",     "https://nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx" },
            { "RecepcaoEvento",       "https://nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx" },
        };

        // ─── Build & Sign ─────────────────────────────────────────────────────────
        public Task<SefazBuildResult> BuildAndSignAsync(FiscalDocument doc, FiscalConfig config)
        {
            try
            {
                // Gera a chave ANTES do XML — QR Code da NFC-e usa doc.AccessKey.
                // Em retry, GenerateAccessKey devolve a chave existente sem gerar nova.
                string accessKey = GenerateAccessKey(doc);
                doc.AccessKey    = accessKey;

                string xml            = BuildNfeXml(doc, config);
                X509Certificate2 cert = LoadCertificate(config);
                string signedXml      = SignXml(xml, cert);

                return Task.FromResult(new SefazBuildResult
                {
                    Success   = true,
                    SignedXml = signedXml,
                    AccessKey = accessKey,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao montar/assinar XML fiscal doc {OriginId}", doc.OriginId);
                return Task.FromResult(new SefazBuildResult { Success = false, ErrorMessage = ex.Message });
            }
        }

        // ─── Transmit ─────────────────────────────────────────────────────────────
        public async Task<SefazTransmitResult> TransmitAsync(string signedXml, FiscalDocument doc, FiscalConfig config)
        {
            try
            {
                X509Certificate2 cert = LoadCertificate(config);
                string envelope       = BuildAuthorizationSoapEnvelope(signedXml);

                logger.LogDebug("SEFAZ Transmit envelope:\n{E}", envelope);

                string response = await PostSoapAsync(
                    GetEndpoint("NFeAutorizacao", config.Environment), envelope, cert,
                    "http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4/nfeAutorizacaoLote");

                logger.LogDebug("SEFAZ Transmit response:\n{R}", response);
                return ParseAuthorizationResponse(response, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha de comunicação SEFAZ doc {OriginId}", doc.OriginId);
                return new SefazTransmitResult { CommunicationError = true, ReturnMessage = ex.Message };
            }
        }

        // ─── Query Status ─────────────────────────────────────────────────────────
        public async Task<SefazTransmitResult> QueryStatusAsync(string accessKeyOrReceipt, FiscalDocument doc, FiscalConfig config)
        {
            try
            {
                X509Certificate2 cert = LoadCertificate(config);
                string envelope       = BuildQuerySoapEnvelope(accessKeyOrReceipt, config);

                string response = await PostSoapAsync(
                    GetEndpoint("NFeConsultaProtocolo", config.Environment), envelope, cert,
                    "http://www.portalfiscal.inf.br/nfe/wsdl/NFeConsultaProtocolo4/nfeConsultaNF");

                logger.LogDebug("SEFAZ Query response:\n{R}", response);
                return ParseAuthorizationResponse(response, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao consultar status doc {AK}", accessKeyOrReceipt);
                return new SefazTransmitResult { CommunicationError = true, ReturnMessage = ex.Message };
            }
        }

        // ─── Cancel ───────────────────────────────────────────────────────────────
        public async Task<SefazEventResult> CancelAsync(FiscalDocument doc, string reason, FiscalConfig config)
        {
            try
            {
                X509Certificate2 cert = LoadCertificate(config);
                string signedEvent    = SignXml(BuildCancellationEvent(doc, reason, config), cert);
                string envelope       = BuildEventSoapEnvelope(signedEvent);

                string response = await PostSoapAsync(
                    GetEndpoint("RecepcaoEvento", config.Environment), envelope, cert,
                    "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4/nfeRecepcaoEvento");

                logger.LogDebug("SEFAZ Cancel response:\n{R}", response);
                return ParseEventResponse(response, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao cancelar NF {AK}", doc.AccessKey);
                return new SefazEventResult { Success = false, ReturnMessage = ex.Message };
            }
        }

        // ─── Correction Letter ────────────────────────────────────────────────────
        public async Task<SefazEventResult> SendCorrectionLetterAsync(FiscalDocument doc, string correctionText, int sequenceNumber, FiscalConfig config)
        {
            try
            {
                X509Certificate2 cert = LoadCertificate(config);
                string signedEvent    = SignXml(BuildCorrectionLetterEvent(doc, correctionText, sequenceNumber, config), cert);
                string envelope       = BuildEventSoapEnvelope(signedEvent);

                string response = await PostSoapAsync(
                    GetEndpoint("RecepcaoEvento", config.Environment), envelope, cert,
                    "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4/nfeRecepcaoEvento");

                logger.LogDebug("SEFAZ CC-e response:\n{R}", response);
                return ParseEventResponse(response, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao enviar CC-e NF {AK}", doc.AccessKey);
                return new SefazEventResult { Success = false, ReturnMessage = ex.Message };
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private string GetEndpoint(string service, string env) =>
            (env == "producao" ? ProducaoEndpoints : HomologacaoEndpoints)[service];

        private static X509Certificate2 LoadCertificate(FiscalConfig config)
        {
            byte[] pfxBytes = Convert.FromBase64String(config.CertificateBase64);
#pragma warning disable SYSLIB0057
            return new X509Certificate2(pfxBytes, config.CertificatePassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
#pragma warning restore SYSLIB0057
        }

        /// <summary>
        /// Assina com RSA-SHA256 + Exclusive C14N (obrigatório pela SEFAZ desde 2015).
        /// SHA-1 gera rejeição código 539. Detecta o atributo Id gerado pelo Unimake
        /// automaticamente (ex: "NFe35...", "ID110111...") — não usa string fixa.
        /// XmlDsigExcC14NUrl não existe como constante no .NET — usa a URL direta.
        /// </summary>
        private static string SignXml(string xml, X509Certificate2 cert)
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(xml);

            var elementToSign = doc.SelectSingleNode("//*[@Id]") as XmlElement
                ?? throw new InvalidOperationException(
                    "Elemento com atributo 'Id' não encontrado no XML. Verifique a serialização do Unimake.");

            string referenceId = elementToSign.GetAttribute("Id");

            var rsa = cert.GetRSAPrivateKey()
                ?? throw new InvalidOperationException("Certificado sem chave RSA privada exportável.");

            var signedXml = new SignedXml(doc) { SigningKey = rsa };

            var reference = new Reference($"#{referenceId}")
            {
                DigestMethod = SignedXml.XmlDsigSHA256Url
            };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            // Exclusive C14N — a constante XmlDsigExcC14NUrl não existe no .NET 9,
            // mas a URL é pública e usada diretamente:
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            signedXml.SignedInfo!.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
            signedXml.SignedInfo.SignatureMethod          = SignedXml.XmlDsigRSASHA256Url;

            signedXml.ComputeSignature();
            doc.DocumentElement!.AppendChild(doc.ImportNode(signedXml.GetXml(), true));

            return doc.OuterXml;
        }

        /// <summary>
        /// Gera chave de acesso de 44 dígitos.
        /// Em retry (doc.AccessKey já preenchido e válido) devolve a mesma chave.
        /// </summary>
        private static string GenerateAccessKey(FiscalDocument doc)
        {
            if (!string.IsNullOrWhiteSpace(doc.AccessKey) && doc.AccessKey.Length == 44)
                return doc.AccessKey;

            string uf     = "35";
            string aamm   = (doc.IssuedAt ?? DateTime.Now).ToString("yyMM");
            string cnpj   = doc.Issuer.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            string mod    = doc.Model.ToString("D2");
            string serie  = doc.Series.ToString("D3");
            string numero = doc.Number.ToString("D9");
            string tpEmis = doc.IsContingency ? "9" : "1";
            string cNF    = Random.Shared.Next(10_000_000, 99_999_999).ToString();

            string key = $"{uf}{aamm}{cnpj}{mod}{serie}{numero}{tpEmis}{cNF}";
            int dv     = CalculateKeyDv(key);
            return $"{key}{dv}";
        }

        private static int CalculateKeyDv(string key)
        {
            int[] weights = [2, 3, 4, 5, 6, 7, 8, 9];
            int sum = 0, wi = 0;
            for (int i = key.Length - 1; i >= 0; i--)
            {
                sum += int.Parse(key[i].ToString()) * weights[wi % 8];
                wi++;
            }
            int rem = sum % 11;
            return rem < 2 ? 0 : 11 - rem;
        }

        // ─── BuildNfeXml ─────────────────────────────────────────────────────────
        private string BuildNfeXml(FiscalDocument doc, FiscalConfig config)
        {
            bool isHomologacao = config.Environment != "producao";
            bool isNfce        = doc.Model == 65;
            int  cityCode      = int.TryParse(doc.Issuer.CityCode, out int cc) ? cc : 3550308;

            var detList = doc.Items.Select((item, i) => new Det
            {
                NItem = i + 1,
                Prod = new Prod
                {
                    CProd    = item.ProductId.Length >= 8 ? item.ProductId[^8..] : item.ProductId.PadLeft(8, '0'),
                    CEAN     = "SEM GTIN",
                    XProd    = item.Description.Length > 120 ? item.Description[..120] : item.Description,
                    NCM      = item.Ncm.Replace(".", ""),
                    CEST     = string.IsNullOrWhiteSpace(item.Cest) ? null : item.Cest,
                    CFOP     = item.Cfop,
                    UCom     = item.UnitOfMeasure,
                    QCom     = item.Quantity,
                    VUnCom   = item.UnitPrice,
                    VProd    = (double)item.Total,       // Unimake usa double
                    CEANTrib = "SEM GTIN",
                    UTrib    = item.UnitOfMeasure,
                    QTrib    = item.Quantity,
                    VUnTrib  = item.UnitPrice,
                    VDesc    = item.Discount > 0 ? (double)item.Discount : 0,
                    IndTot   = SimNao.Sim,               // Unimake usa SimNao, não IndicadorTotal
                },
                Imposto = new Imposto
                {
                    ICMS = new ICMS
                    {
                        // ICMSSN400 não existe na Unimake — ICMSSN102 é o equivalente
                        // para Simples Nacional sem crédito (CSOSN 102/400/500).
                        // CSOSN é string na Unimake (não enum).
                        ICMSSN102 = new ICMSSN102
                        {
                            Orig  = item.IcmsOrigin == "0"
                                ? OrigemMercadoria.Nacional
                                : OrigemMercadoria.Estrangeira,
                            CSOSN = "400",
                        }
                    },
                    PIS = new PIS
                    {
                        // CST é string (não enum CSTPIS).
                        // Propriedade é PPIS (não PAliq).
                        PISOutr = new PISOutr
                        {
                            CST  = "07",
                            VBC  = 0,
                            PPIS = 0,
                            VPIS = 0,
                        }
                    },
                    COFINS = new COFINS
                    {
                        // Propriedade é PCOFINS (não PAliq).
                        COFINSOutr = new COFINSOutr
                        {
                            CST     = "07",
                            VBC     = 0,
                            PCOFINS = 0,
                            VCOFINS = 0,
                        }
                    }
                }
            }).ToList();

            var nfe = new NFe
            {
                InfNFe = [new InfNFe
                {
                    Versao = "4.00",

                    Ide = new Ide
                    {
                        CUF      = UFBrasil.SP,
                        NatOp    = isNfce ? "VENDA A CONSUMIDOR" : "VENDA DE MERCADORIA",
                        Mod      = isNfce ? ModeloDFe.NFCe : ModeloDFe.NFe,
                        Serie    = doc.Series,
                        NNF      = (int)doc.Number,
                        DhEmi    = doc.IssuedAt.HasValue
                                    ? new DateTimeOffset(doc.IssuedAt.Value, TimeSpan.FromHours(-3))
                                    : DateTimeOffset.Now,
                        // TpNF é TipoOperacao (não TipoNF)
                        TpNF     = TipoOperacao.Saida,
                        // IdDest é DestinoOperacao (não IdentificacaoDestinatario)
                        IdDest   = DestinoOperacao.OperacaoInterna,
                        CMunFG   = cityCode,
                        // TpImp é FormatoImpressaoDANFE (não TipoImpressaoDANFE)
                        TpImp    = isNfce ? FormatoImpressaoDANFE.NFCe : FormatoImpressaoDANFE.NormalRetrato,
                        // TipoEmissao: Normal (não EmissaoNormal), ContingenciaOffLine (não ContingenciaOffLineCFeSAT)
                        TpEmis   = doc.IsContingency ? TipoEmissao.ContingenciaOffLine : TipoEmissao.Normal,
                        TpAmb    = isHomologacao ? TipoAmbiente.Homologacao : TipoAmbiente.Producao,
                        FinNFe   = FinalidadeNFe.Normal,
                        // IndFinal é SimNao (não IndicadorConsumidorFinal)
                        IndFinal = SimNao.Sim,
                        // IndPres é IndicadorPresenca (não IndicadorPresencaComprador)
                        IndPres  = IndicadorPresenca.OperacaoPresencial,
                    },

                    Emit = new Emit
                    {
                        CNPJ  = doc.Issuer.Cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                        XNome = doc.Issuer.CorporateName,
                        XFant = doc.Issuer.TradeName,
                        IE    = doc.Issuer.StateRegistration,
                        CRT   = doc.Issuer.TaxRegime switch
                        {
                            2 => CRT.SimplesNacionalExcessoSublimite,
                            3 => CRT.RegimeNormal,
                            _ => CRT.SimplesNacional,
                        },
                        EnderEmit = new EnderEmit
                        {
                            XLgr    = doc.Issuer.Street,
                            Nro     = doc.Issuer.Number,
                            XBairro = doc.Issuer.District,
                            CMun    = cityCode,
                            XMun    = doc.Issuer.City,
                            UF      = UFBrasil.SP,
                            CEP     = doc.Issuer.ZipCode.Replace("-", ""),
                            Fone    = doc.Issuer.Phone
                                        .Replace("(", "").Replace(")", "")
                                        .Replace(" ", "").Replace("-", ""),
                        }
                    },

                    Dest = isNfce && string.IsNullOrWhiteSpace(doc.Recipient.Document)
                        ? null
                        : BuildDest(doc, isHomologacao),

                    Det = detList,

                    Total = new Total
                    {
                        ICMSTot = new ICMSTot
                        {
                            VBC        = 0,
                            VICMS      = 0,
                            VICMSDeson = 0,
                            VFCP       = 0,
                            VBCST      = 0,
                            VST        = 0,
                            VFCPST     = 0,
                            VFCPSTRet  = 0,
                            VProd      = (double)doc.Totals.ProductsTotal,
                            VFrete     = (double)doc.Totals.Freight,
                            VSeg       = 0,
                            VDesc      = (double)doc.Totals.Discount,
                            VII        = 0,
                            VIPI       = 0,
                            VIPIDevol  = 0,
                            VPIS       = (double)doc.Totals.PisValue,
                            VCOFINS    = (double)doc.Totals.CofinsValue,
                            VOutro     = 0,
                            VNF        = (double)doc.Totals.GrandTotal,
                        }
                    },

                    Transp = new Transp
                    {
                        ModFrete = ModalidadeFrete.SemOcorrenciaTransporte,
                    },

                    Pag = new Pag
                    {
                        DetPag = [new DetPag
                        {
                            TPag = MapPaymentMethod(doc.PaymentMethodName),
                            VPag = (double)doc.Totals.GrandTotal,
                        }]
                    },
                    InfAdic = string.IsNullOrWhiteSpace(doc.AdditionalInfo) ? null : new InfAdic
                    {
                        InfCpl = doc.AdditionalInfo,
                    }
                }],
                InfNFeSupl = isNfce ? new InfNFeSupl
                {
                    QrCode   = BuildQrCodeNfce(doc, config),
                    UrlChave = config.Environment == "producao" ? "https://www.nfce.fazenda.sp.gov.br/consulta" : "https://www.homologacao.nfce.fazenda.sp.gov.br/consulta",
                } : null,
            };

            // XMLUtility.Serializar retorna XmlDocument na Unimake — converter para string
            var result = XMLUtility.Serializar(nfe);
            return result is XmlDocument xd ? xd.OuterXml : result.ToString()!;
        }

        private static Dest BuildDest(FiscalDocument doc, bool isHomologacao)
        {
            string docNum = doc.Recipient.Document.Replace(".", "").Replace("-", "").Replace("/", "");

            var dest = new Dest
            {
                XNome     = isHomologacao
                    ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL"
                    : doc.Recipient.Name,
                // IndIEDest é IndicadorIEDestinatario na Unimake (não IndIEDest como tipo)
                IndIEDest = (IndicadorIEDestinatario)doc.Recipient.IeIndicator,
                Email     = doc.Recipient.Email,
            };

            if (docNum.Length == 11)      dest.CPF  = docNum;
            else if (docNum.Length == 14) dest.CNPJ = docNum;

            if (!string.IsNullOrWhiteSpace(doc.Recipient.StateRegistration))
                dest.IE = doc.Recipient.StateRegistration;

            return dest;
        }

        // PIX = PagamentoInstantaneo (valor 17) na Unimake — não existe MeioPagamento.PIX
        private static MeioPagamento MapPaymentMethod(string? methodName) =>
            methodName?.ToLowerInvariant() switch
            {
                var m when m != null && m.Contains("pix")      => MeioPagamento.PagamentoInstantaneo,
                var m when m != null && m.Contains("débit")    => MeioPagamento.CartaoDebito,
                var m when m != null && m.Contains("debit")    => MeioPagamento.CartaoDebito,
                var m when m != null && m.Contains("crédit")   => MeioPagamento.CartaoCredito,
                var m when m != null && m.Contains("credit")   => MeioPagamento.CartaoCredito,
                var m when m != null && m.Contains("boleto")   => MeioPagamento.BoletoBancario,
                var m when m != null && m.Contains("cheque")   => MeioPagamento.Cheque,
                var m when m != null && m.Contains("convenio") => MeioPagamento.CreditoLoja,
                _                                               => MeioPagamento.Dinheiro,
            };

        private static string BuildQrCodeNfce(FiscalDocument doc, FiscalConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Csc)) return string.Empty;

            string tpAmb   = config.Environment == "producao" ? "1" : "2";
            string baseUrl = config.Environment == "producao"
                ? "https://www.nfce.fazenda.sp.gov.br/qrcode"
                : "https://www.homologacao.nfce.fazenda.sp.gov.br/qrcode";

            string dhEmi     = Uri.EscapeDataString((doc.IssuedAt ?? DateTime.Now).ToString("yyyy-MM-ddTHH:mm:sszzz"));
            string hashInput = $"{doc.AccessKey}|2|{tpAmb}|{config.CscId}|{config.Csc}";
            string hash      = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(hashInput))).ToLower();

            return $"{baseUrl}?chNFe={doc.AccessKey}&nVersao=100&tpAmb={tpAmb}&cDest=&dhEmi={dhEmi}" +
                   $"&vNF={doc.Totals.GrandTotal:F2}&vICMS=0.00&digVal={Uri.EscapeDataString(hash)}&cIdToken={config.CscId}";
        }

        // ─── SOAP Envelopes ───────────────────────────────────────────────────────

        private static string StripXmlDeclaration(string xml) =>
            xml.TrimStart().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)
                ? xml[(xml.IndexOf("?>", StringComparison.Ordinal) + 2)..].TrimStart()
                : xml;

        private static string BuildAuthorizationSoapEnvelope(string signedXml)
        {
            string nfeBody = StripXmlDeclaration(signedXml);
            return $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                  <soap12:Body>
                    <nfeDadosMsg xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4">
                      <enviNFe versao="4.00" xmlns="http://www.portalfiscal.inf.br/nfe">
                        <idLote>{DateTime.UtcNow:yyyyMMddHHmmssfff}</idLote>
                        <indSinc>1</indSinc>
                        {nfeBody}
                      </enviNFe>
                    </nfeDadosMsg>
                  </soap12:Body>
                </soap12:Envelope>
                """;
        }

        private static string BuildQuerySoapEnvelope(string accessKey, FiscalConfig config) => $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                             xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                             xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
              <soap12:Body>
                <nfeDadosMsg xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeConsultaProtocolo4">
                  <consSitNFe versao="4.00" xmlns="http://www.portalfiscal.inf.br/nfe">
                    <tpAmb>{(config.Environment == "producao" ? "1" : "2")}</tpAmb>
                    <xServ>CONSULTAR</xServ>
                    <chNFe>{accessKey}</chNFe>
                  </consSitNFe>
                </nfeDadosMsg>
              </soap12:Body>
            </soap12:Envelope>
            """;

        private static string BuildCancellationEvent(FiscalDocument doc, string reason, FiscalConfig config)
        {
            string cnpj     = doc.Issuer.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            string dhEvento = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            return $"""
            <envEvento versao="1.00" xmlns="http://www.portalfiscal.inf.br/nfe">
              <idLote>{DateTime.UtcNow:yyyyMMddHHmmssfff}</idLote>
              <evento versao="1.00">
                <infEvento Id="ID110111{doc.AccessKey}01">
                  <cOrgao>35</cOrgao>
                  <tpAmb>{(config.Environment == "producao" ? "1" : "2")}</tpAmb>
                  <CNPJ>{cnpj}</CNPJ>
                  <chNFe>{doc.AccessKey}</chNFe>
                  <dhEvento>{dhEvento}</dhEvento>
                  <tpEvento>110111</tpEvento>
                  <nSeqEvento>1</nSeqEvento>
                  <verEvento>1.00</verEvento>
                  <detEvento versao="1.00">
                    <descEvento>Cancelamento</descEvento>
                    <nProt>{doc.Protocol}</nProt>
                    <xJust>{reason}</xJust>
                  </detEvento>
                </infEvento>
              </evento>
            </envEvento>
            """;
        }

        private static string BuildCorrectionLetterEvent(FiscalDocument doc, string text, int seq, FiscalConfig config)
        {
            string cnpj     = doc.Issuer.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            string dhEvento = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            return $"""
            <envEvento versao="1.00" xmlns="http://www.portalfiscal.inf.br/nfe">
              <idLote>{DateTime.UtcNow:yyyyMMddHHmmssfff}</idLote>
              <evento versao="1.00">
                <infEvento Id="ID110110{doc.AccessKey}{seq:D2}">
                  <cOrgao>35</cOrgao>
                  <tpAmb>{(config.Environment == "producao" ? "1" : "2")}</tpAmb>
                  <CNPJ>{cnpj}</CNPJ>
                  <chNFe>{doc.AccessKey}</chNFe>
                  <dhEvento>{dhEvento}</dhEvento>
                  <tpEvento>110110</tpEvento>
                  <nSeqEvento>{seq}</nSeqEvento>
                  <verEvento>1.00</verEvento>
                  <detEvento versao="1.00">
                    <descEvento>Carta de Correcao</descEvento>
                    <xCorrecao>{text}</xCorrecao>
                    <xCondUso>A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.</xCondUso>
                  </detEvento>
                </infEvento>
              </evento>
            </envEvento>
            """;
        }

        private static string BuildEventSoapEnvelope(string signedEvent)
        {
            string body = StripXmlDeclaration(signedEvent);
            return $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                  <soap12:Body>
                    <nfeDadosMsg xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4">
                      {body}
                    </nfeDadosMsg>
                  </soap12:Body>
                </soap12:Envelope>
                """;
        }

        private static async Task<string> PostSoapAsync(string url, string soapBody, X509Certificate2 cert, string action)
        {
            using var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var client = new HttpClient(handler, disposeHandler: false) { Timeout = TimeSpan.FromSeconds(30) };
            var content = new StringContent(soapBody, Encoding.UTF8,
                $"application/soap+xml; charset=utf-8; action=\"{action}\"");

            var httpResponse = await client.PostAsync(url, content);
            string body      = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"SEFAZ HTTP {(int)httpResponse.StatusCode}: {body[..Math.Min(300, body.Length)]}");

            return body;
        }

        // ─── Parse Responses ──────────────────────────────────────────────────────

        private static SefazTransmitResult ParseAuthorizationResponse(string xml, ILogger logger)
        {
            var doc = new XmlDocument();
            try { doc.LoadXml(xml); }
            catch (XmlException ex)
            {
                logger.LogError(ex, "Resposta SEFAZ não é XML. Conteúdo: {X}", xml[..Math.Min(500, xml.Length)]);
                return new SefazTransmitResult
                {
                    CommunicationError = true,
                    ReturnMessage = $"Resposta inválida da SEFAZ: {xml[..Math.Min(200, xml.Length)]}"
                };
            }

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            ns.AddNamespace("nfe",  "http://www.portalfiscal.inf.br/nfe");

            string? cStat   = doc.SelectSingleNode("//nfe:cStat",   ns)?.InnerText;
            string? xMotivo = doc.SelectSingleNode("//nfe:xMotivo", ns)?.InnerText;
            string? nProt   = doc.SelectSingleNode("//nfe:nProt",   ns)?.InnerText;
            string? nRec    = doc.SelectSingleNode("//nfe:nRec",    ns)?.InnerText;

            if (cStat is null)
            {
                logger.LogWarning("Resposta SEFAZ sem cStat. XML: {X}", xml);
                return new SefazTransmitResult
                {
                    CommunicationError = true,
                    ReturnMessage = "SEFAZ não retornou cStat. Verifique o log."
                };
            }

            return new SefazTransmitResult
            {
                Authorized    = cStat == "100",
                Rejected      = cStat != "100" && cStat != "110"
                                && !cStat.StartsWith('1') && !cStat.StartsWith('5'),
                Denied        = cStat == "110",
                ReturnCode    = cStat,
                ReturnMessage = xMotivo ?? "",
                Protocol      = nProt ?? "",
                ReceiptNumber = nRec ?? "",
                AuthorizedXml = cStat == "100" ? xml : "",
            };
        }

        private static SefazEventResult ParseEventResponse(string xml, ILogger logger)
        {
            var doc = new XmlDocument();
            try { doc.LoadXml(xml); }
            catch (XmlException ex)
            {
                logger.LogError(ex, "Resposta SEFAZ (evento) não é XML. Conteúdo: {X}", xml[..Math.Min(500, xml.Length)]);
                return new SefazEventResult { Success = false, ReturnMessage = "Resposta inválida da SEFAZ." };
            }

            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            ns.AddNamespace("nfe",  "http://www.portalfiscal.inf.br/nfe");

            string? cStat   = doc.SelectSingleNode("//nfe:cStat",   ns)?.InnerText;
            string? xMotivo = doc.SelectSingleNode("//nfe:xMotivo", ns)?.InnerText;
            string? nProt   = doc.SelectSingleNode("//nfe:nProt",   ns)?.InnerText;

            if (cStat is null)
                logger.LogWarning("Resposta SEFAZ (evento) sem cStat. XML: {X}", xml);

            return new SefazEventResult
            {
                Success       = cStat == "135",
                Protocol      = nProt ?? "",
                EventXml      = xml,
                ReturnCode    = cStat ?? "",
                ReturnMessage = xMotivo ?? "",
            };
        }
    }
}