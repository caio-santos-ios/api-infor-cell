using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController(ICompanyService service) : ControllerBase
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

        // [Authorize]
        // [HttpGet("select")]
        // public async Task<IActionResult> GetSelect()
        // {
        //     ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
        //     return StatusCode(response.StatusCode, new { response.Message, response.Result });
        // }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Company?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Company?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpPut("logo")]
        public async Task<IActionResult> SavePhotoProfileAsync([FromForm] SaveCompanyPhotoDTO body)
        {
            ResponseApi<Company?> response = await service.SavePhotoProfileAsync(body);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Company> response = await service.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}