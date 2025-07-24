using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/equipamentos/importacao")]
    [ApiController]
    public class EquipamentoImportacaoController : ControllerBase
    {
        private readonly IEquipamentoImportacaoService _service;

        public EquipamentoImportacaoController(IEquipamentoImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<EquipamentoDto>>> PreviewExcelEquipamentos(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var equipamentos = await _service.CarregarPreviewAsync(stream);
            return Ok(equipamentos);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarEquipamentos([FromBody] List<EquipamentoDto> equipamentos)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarEquipamentosAsync(equipamentos, usuario);
            return Ok(erros);
        }
    }

}
