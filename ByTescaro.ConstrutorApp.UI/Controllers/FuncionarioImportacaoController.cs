using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/funcionarios/importacao")]
    [ApiController]
    public class FuncionarioImportacaoController : ControllerBase
    {
        private readonly IFuncionarioImportacaoService _service;

        public FuncionarioImportacaoController(IFuncionarioImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<FuncionarioDto>>> PreviewExcelFuncionarios(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var funcionarios = await _service.CarregarPreviewAsync(stream);
            return Ok(funcionarios);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarFuncionarios([FromBody] List<FuncionarioDto> funcionarios)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarFuncionariosAsync(funcionarios, usuario);
            return Ok(erros);
        }
    }

}
