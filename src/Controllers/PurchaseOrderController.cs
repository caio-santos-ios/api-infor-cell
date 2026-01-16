using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/purchase-orders")]
    [ApiController]
    public class PurchaseOrderController(IPurchaseOrderService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<PurchaseOrder?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePurchaseOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<PurchaseOrder?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut("approval")]
        public async Task<IActionResult> UpdateApproval([FromBody] UpdatePurchaseOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<PurchaseOrder?> response = await service.UpdateApprovalAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<PurchaseOrder> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}