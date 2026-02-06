using System.Security.Claims;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController(IEmployeeService service) : ControllerBase
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
        [HttpGet("select/sellers")]
        public async Task<IActionResult> GetSelectSellers([FromQuery] string plan, [FromQuery] string company, [FromQuery] string store)
        {
            ResponseApi<List<Employee>> response = await service.GetSellersAsync(plan, company, store);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Employee?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Employee?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("modules")]
        public async Task<IActionResult> UpdateModules([FromBody] UpdateModuleEmployeeDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<Employee?> response = await service.UpdateModuleAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpPut("calendar")]
        public async Task<IActionResult> UpdateCalendar([FromBody] UpdateCalendarEmployeeDTO request)
        {
            if (request == null) return BadRequest("Dados inválidos.");

            ResponseApi<Employee?> response = await service.UpdateCalendarAsync(request);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpGet("logged")]
        public async Task<IActionResult> GetLoggedAsync()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ResponseApi<dynamic?> response = await service.GetLoggedAsync(userId!);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Employee> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}