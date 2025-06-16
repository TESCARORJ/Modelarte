using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/servicos/importacao")]
    [ApiController]
    public class ServicoImportacaoController : ControllerBase
    {
        private readonly IServicoImportacaoService _service;

        public ServicoImportacaoController(IServicoImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<ServicoDto>>> PreviewExcelServicos(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var servicos = await _service.CarregarPreviewAsync(stream);
            return Ok(servicos);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarServicos([FromBody] List<ServicoDto> servicos)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarServicosAsync(servicos, usuario);
            return Ok(erros);
        }
    }

}
