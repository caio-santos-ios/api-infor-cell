using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_infor_cell.src.Controllers
{
    [Route("api/receita-ws")]
    [ApiController]
    public class ReceitaWSController() : ControllerBase
    {
        [Authorize]
        [HttpGet("{cnpj}")]
        public async Task<IActionResult> GetAll(string cnpj)
        {
            using var http = new HttpClient();

            var response = await http.GetAsync(
                $"https://www.receitaws.com.br/v1/cnpj/{cnpj}"
            );

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}