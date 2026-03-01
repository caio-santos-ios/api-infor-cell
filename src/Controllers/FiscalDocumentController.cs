using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/fiscal")]
    [ApiController]
    public class FiscalDocumentController(IFiscalDocumentService service) : ControllerBase
    {
        /// <summary>
        /// Emite NF-e (55) ou NFC-e (65) a partir de uma venda ou OS.
        /// Body: { originId, originType: "sales_order"|"service_order", model: 55|65 }
        /// </summary>
        [Authorize]
        [HttpPost("emit")]
        public async Task<IActionResult> Emit([FromBody] EmitFiscalDocumentDTO body)
        {
            ResponseApi<FiscalDocument?> response = await service.EmitAsync(body, User.FindFirst("id")?.Value ?? "");
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Cancela uma NF-e/NFC-e autorizada. Requer motivo (mín. 15 chars).</summary>
        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelFiscalDocumentDTO body)
        {
            ResponseApi<FiscalDocument?> response = await service.CancelAsync(body, User.FindFirst("id")?.Value ?? "");
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Envia Carta de Correção Eletrônica (CC-e).</summary>
        [Authorize]
        [HttpPost("correction-letter")]
        public async Task<IActionResult> CorrectionLetter([FromBody] CorrectionLetterDTO body)
        {
            ResponseApi<FiscalEvent?> response = await service.SendCorrectionLetterAsync(body, User.FindFirst("id")?.Value ?? "");
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Reprocessa documentos com status Rejected ou Contingency.</summary>
        [Authorize]
        [HttpPost("retry/{id}")]
        public async Task<IActionResult> Retry(string id)
        {
            ResponseApi<FiscalDocument?> response = await service.RetryAsync(id, User.FindFirst("id")?.Value ?? "");
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Busca o documento fiscal de uma venda ou OS. ?originId=X&originType=sales_order</summary>
        [Authorize]
        [HttpGet("by-origin")]
        public async Task<IActionResult> GetByOrigin([FromQuery] string originId, [FromQuery] string originType)
        {
            ResponseApi<dynamic?> response = await service.GetByOriginAsync(originId, originType);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Lista todos os documentos fiscais com paginação e filtros.</summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Salva configuração fiscal da loja (certificado, série, CFOP, ambiente).</summary>
        [Authorize]
        [HttpPost("config")]
        public async Task<IActionResult> SaveConfig([FromBody] SaveFiscalConfigDTO body)
        {
            ResponseApi<FiscalConfig?> response = await service.SaveConfigAsync(body, User.FindFirst("id")?.Value ?? "");
            return StatusCode(response.StatusCode, new { response.Result });
        }

        /// <summary>Busca configuração fiscal da loja.</summary>
        [Authorize]
        [HttpGet("config/{storeId}")]
        public async Task<IActionResult> GetConfig(string storeId)
        {
            ResponseApi<FiscalConfig?> response = await service.GetConfigAsync(storeId);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}