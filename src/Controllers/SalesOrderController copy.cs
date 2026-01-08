using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/salesOrders")]
    [ApiController]
    public class SalesOrderController(ISalesOrderService sales) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await sales.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await sales.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        // [Authorize]
        // [HttpGet("select")]
        // public async Task<IActionResult> GetSelect()
        // {
        //     ResponseApi<List<dynamic>> response = await sales.GetSelectAsync(new(Request.Query));
        //     return StatusCode(response.StatusCode, new { response.Message, response.Result });
        // }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSalesOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<SalesOrder?> response = await sales.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSalesOrderDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<SalesOrder?> response = await sales.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<SalesOrder> response = await sales.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}