using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        private string Plan    => User.FindFirst("plan")?.Value    ?? string.Empty;
        private string Company => User.FindFirst("company")?.Value ?? string.Empty;
        private string Store   => User.FindFirst("store")?.Value   ?? string.Empty;

        [Authorize]
        [HttpGet("cards")]
        public async Task<IActionResult> GetCards([FromQuery] string selectedStore)
        {
            ResponseApi<dynamic?> response = await service.GetCardsAsync(Plan, Company, selectedStore);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySales([FromQuery] string selectedStore)
        {
            ResponseApi<dynamic?> response = await service.GetMonthlySalesAsync(Plan, Company, selectedStore);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("monthly-target")]
        public async Task<IActionResult> GetMonthlyTarget([FromQuery] string selectedStore)
        {
            ResponseApi<dynamic?> response = await service.GetMonthlyTargetAsync(Plan, Company, selectedStore);
            return StatusCode(response.StatusCode, new { response.Result });
        }

        [Authorize]
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] string selectedStore)
        {
            ResponseApi<dynamic?> response = await service.GetRecentOrdersAsync(Plan, Company, selectedStore);
            return StatusCode(response.StatusCode, new { response.Result });
        }
    }
}