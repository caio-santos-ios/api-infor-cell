using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/check")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return StatusCode(200, new { Message = "API OK!" });
        }
    }
}