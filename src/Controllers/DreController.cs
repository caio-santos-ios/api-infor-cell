using api_infor_cell.src.Interfaces;
using api_infor_cell.src.Models.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/dre")]
    [ApiController]
    public class DreController(IDreService service) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Generate(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string regime = "competencia"
        )
        {
            string plan    = User.FindFirst("plan")?.Value    ?? string.Empty;
            string company = User.FindFirst("company")?.Value ?? string.Empty;
            string store   = User.FindFirst("store")?.Value   ?? string.Empty;

            if (!DateTime.TryParse(startDate, out DateTime start))
            {
                return BadRequest(new { message = "Data inicial inválida" });
            }

            if (!DateTime.TryParse(endDate, out DateTime end))
            {
                return BadRequest(new { message = "Data final inválida" });
            }

            ResponseApi<dynamic?> response = await service.GenerateAsync(plan, company, store, start, end, regime);
            return StatusCode(response.StatusCode, new { response.Message, response.Result });
        }
    }
}