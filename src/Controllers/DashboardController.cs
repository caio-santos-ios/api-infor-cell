using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models;
using api_infor_cell.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        [Authorize]
        [HttpGet("first-card")]
        public async Task<IActionResult> GetCard()
        {
            ResponseApi<dynamic?> response = await service.GetFirstCardAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
        
        [Authorize]
        [HttpGet("recent-patients")]
        public async Task<IActionResult> GetRecentPatient()
        {
            ResponseApi<List<dynamic>> response = await service.GetRecentPatientAsync();
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}