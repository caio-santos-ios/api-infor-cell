using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using api_infor_cell.src.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    public class AddressController(IAddressService addressService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            PaginationApi<List<dynamic>> response = await addressService.GetAllAsync(new(Request.Query));
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            ResponseApi<dynamic?> response = await addressService.GetByIdAggregateAsync(id);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("{parentId}/{parent}")]
        public async Task<IActionResult> GetByParentId(string parentId, string parent)
        {
            ResponseApi<Address?> response = await addressService.GetByParentIdAsync(parentId, parent);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDTO address)
        {
            if (address == null) return BadRequest("Dados inválidos.");

            ResponseApi<Address?> response = await addressService.CreateAsync(address);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAddressDTO address)
        {
            if (address == null) return BadRequest("Dados inválidos.");

            ResponseApi<Address?> response = await addressService.UpdateAsync(address);

            return StatusCode(response.StatusCode, new { response.Message });
        }
        
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseApi<Address> response = await addressService.DeleteAsync(id);

            return StatusCode(response.StatusCode, new { response.Message });
        }
    }
}