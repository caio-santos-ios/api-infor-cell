using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController(IStockService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await service.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            string? plan = User.FindFirst("plan")?.Value;
            string? company = User.FindFirst("company")?.Value;
            
            ResponseApi<List<dynamic>> response = await service.GetByProductIdAggregationAsync(plan!, company!, productId);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await service.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        // [Authorize]
        // [HttpGet("select")]
        // public async Task<IActionResult> GetSelect()
        // {
        //     ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
        //     return StatusCode(response.StatusCode, new { response.Message, response.Result });
        // }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Stock?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateStockDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Stock?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Stock> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}