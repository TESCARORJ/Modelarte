using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/clientes/importacao")]
    [ApiController]
    public class ClienteImportacaoController : ControllerBase
    {
        private readonly IClienteImportacaoService _service;

        public ClienteImportacaoController(IClienteImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<ClienteDto>>> PreviewExcelClientes(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var clientes = await _service.CarregarPreviewAsync(stream);
            return Ok(clientes);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarClientes([FromBody] List<ClienteDto> clientes)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarClientesAsync(clientes, usuario);
            return Ok(erros);
        }
    }

}
