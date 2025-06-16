using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraFuncionarioController : ControllerBase
    {
        private readonly IObraFuncionarioService _service;

        public ObraFuncionarioController(IObraFuncionarioService service)
        {
            _service = service;
        }

        [HttpGet("{obraId}")]
        public async Task<IActionResult> GetByObraId(long obraId)
        {
            var lista = await _service.ObterPorObraIdAsync(obraId);
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraFuncionarioDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpDelete("{funcionarioId}")]
        public async Task<IActionResult> Delete(long funcionarioId)
        {
            await _service.RemoverAsync( funcionarioId);
            return Ok();
        }

        [HttpGet("disponiveis/{obraId}")]
        public async Task<IActionResult> GetFuncionariosDisponiveis(long obraId)
        {
            var disponiveis = await _service.ObterFuncionariosDisponiveisAsync(obraId);
            return Ok(disponiveis);
        }


        [HttpGet("total-disponiveis")]
        public async Task<IActionResult> GetFuncionariosTotalDisponiveisAsync()
        {
            var disponiveis = await _service.ObterFuncionariosTotalDisponiveisAsync();
            return Ok(disponiveis);
        }

        [HttpGet("total-alocados")]
        public async Task<IActionResult> GetFuncionariosAlocadosAsync()
        {
            var disponiveis = await _service.ObterFuncionariosTotalAlocadosAsync();
            return Ok(disponiveis);
        }
    }

}
