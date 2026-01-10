using System.Security.Claims;
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
    [Route("api/stores")]
    [ApiController]
    public class StoreController(IStoreService service) : ControllerBase
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
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if(!new List<string>{"Admin", "Master"}.Contains(userRole!))
            {
                string? storesId = User.FindFirst("stores")?.Value;
                if(storesId is not null)
                {
                    var queryItems = Request.Query.ToDictionary(x => x.Key, x => x.Value);
                    List<string> storesList = JsonSerializer.Deserialize<List<string>>(storesId)!;
                    string stores = string.Join(",", storesList);

                    queryItems["in$id"] = stores;
                    Request.Query = new QueryCollection(queryItems);
                }
            }

            ResponseApi<List<dynamic>> response = await service.GetSelectAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStoreDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Store?> response = await service.CreateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateStoreDTO body)
        {
            if (body == null) return BadRequest("Dados inválidos.");

            ResponseApi<Store?> response = await service.UpdateAsync(body);

            return StatusCode(response.StatusCode, new { response.Result });
        }        
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            string? plan = User.FindFirst("plan")?.Value;
            string? company = User.FindFirst("company")?.Value;

            ResponseApi<Store> response = await service.DeleteAsync(id, plan!, company!);

            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}