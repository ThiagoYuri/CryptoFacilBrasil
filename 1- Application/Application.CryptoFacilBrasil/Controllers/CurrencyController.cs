using Application.CryptoFacilBrasil.ModelsRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Application.CryptoFacilBrasil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[EnableCors("AllowAll")]
    public class CurrencyController : ControllerBase
    {
        [HttpPost("currencyUpdate")]
       // [EnableCors("AllowAll")]
        public async Task<IActionResult> UpdateCurrencies([FromBody]List<UpdateCurrenciesRequest> currencies)
        {
            try
            {
                // Verifica se a lista está vazia
                if (currencies == null || !currencies.Any())
                {
                    return BadRequest("Currency data cannot be empty.");
                }

                // Caminho do arquivo JSON
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "currencies.json");


                // Serializa os dados para o formato JSON
                var jsonData = System.Text.Json.JsonSerializer.Serialize(currencies);

                // Garante que o diretório existe
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // Salva o JSON no caminho especificado
                await System.IO.File.WriteAllTextAsync(filePath, jsonData);

                return Ok("JSON file updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("getCurrency")]
        [AllowAnonymous]
        public async Task<IActionResult> getCurrency()
        {
            try
            {
                // Caminho do arquivo JSON
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "currencies.json");

                // Verifica se o arquivo existe
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Currency data file not found.");
                }

                // Lê o conteúdo do arquivo JSON
                var jsonData = await System.IO.File.ReadAllTextAsync(filePath);


                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
