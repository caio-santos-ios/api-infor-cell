using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/sales-orders")]
    [ApiController]
    public class SalesOrderController(ISalesOrderService service) : ControllerBase
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
        public async Task<IActionResult> Create([FromBody] CreateSalesOrderDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<SalesOrder?> response = await service.CreateAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSalesOrderDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<SalesOrder?> response = await service.UpdateAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpPut("finish")]
        public async Task<IActionResult> Finish([FromBody] FinishSalesOrderDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<SalesOrder?> response = await service.FinishAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<SalesOrder> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}