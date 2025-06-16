using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/insumos/importacao")]
    [ApiController]
    public class InsumoImportacaoController : ControllerBase
    {
        private readonly IInsumoImportacaoService _service;

        public InsumoImportacaoController(IInsumoImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<InsumoDto>>> PreviewExcelInsumos(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var insumos = await _service.CarregarPreviewAsync(stream);
            return Ok(insumos);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarInsumos([FromBody] List<InsumoDto> insumos)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarInsumosAsync(insumos, usuario);
            return Ok(erros);
        }
    }

}
