using System.Security.Claims;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/boxes")]
    [ApiController]
    public class BoxController(IBoxService service) : ControllerBase
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
        [HttpGet("verify")]
        public async Task<IActionResult> GetVerifyAsync()
        {
            string? createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            ResponseApi<dynamic?> response = await service.GetByCreatedIdAggregateAsync(createdBy!);
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoxDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Box?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateBoxDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Box?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Box> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}