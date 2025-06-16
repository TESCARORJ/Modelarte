using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("api/fornecedores/importacao")]
    [ApiController]
    public class FornecedorImportacaoController : ControllerBase
    {
        private readonly IFornecedorImportacaoService _service;

        public FornecedorImportacaoController(IFornecedorImportacaoService service)
        {
            _service = service;
        }

        [HttpPost("preview")]
        public async Task<ActionResult<List<FornecedorDto>>> PreviewExcelFornecedores(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var stream = file.OpenReadStream();
            var fornecedores = await _service.CarregarPreviewAsync(stream);
            return Ok(fornecedores);
        }

        [HttpPost("importar")]
        public async Task<ActionResult<List<ErroImportacaoDto>>> ImportarFornecedores([FromBody] List<FornecedorDto> fornecedores)
        {
            var usuario = User?.Identity?.Name ?? "Importador";
            var erros = await _service.ImportarFornecedoresAsync(fornecedores, usuario);
            return Ok(erros);
        }
    }

}
