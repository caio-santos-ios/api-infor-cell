using System.Text.Json;
using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using api_infor_cell.src.Shared.Utils;
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
            string? plan = User.FindFirst("plan")?.Value;
            if (plan is not null)
            {
                var queryItems = Request.Query.ToDictionary(x => x.Key, x => x.Value);

                if(plan is not null) 
                {
                    queryItems["plan"] = plan;
                    Request.Query = new QueryCollection(queryItems);
                }
            }

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
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            string? companiesId = User.FindFirst("companies")?.Value;
            if (companiesId is not null)
            {
                var queryItems = Request.Query.ToDictionary(x => x.Key, x => x.Value);
                
                List<string>? companies = JsonSerializer.Deserialize<List<string>>(companiesId);

                if(companies is not null) 
                {
                    queryItems["in$id"] = new Microsoft.Extensions.Primitives.StringValues(string.Join(", ", companies));
                    Request.Query = new QueryCollection(queryItems);
                }
            };

            ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
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
            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string? plan = User.FindFirst("plan")?.Value;

            ResponseApi<Company> response = await service.DeleteAsync(id, plan!);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}