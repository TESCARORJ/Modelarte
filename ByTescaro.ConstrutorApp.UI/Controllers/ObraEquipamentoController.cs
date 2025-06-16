using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraEquipamentoController : ControllerBase
    {
        private readonly IObraEquipamentoService _service;

        public ObraEquipamentoController(IObraEquipamentoService service)
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
        public async Task<IActionResult> Post([FromBody] ObraEquipamentoDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpDelete("{equipamentoId}")]
        public async Task<IActionResult> Delete(long equipamentoId)
        {
            await _service.RemoverAsync( equipamentoId);
            return Ok();
        }

        [HttpGet("disponiveis/{obraId}")]
        public async Task<IActionResult> GetEquipamentosDisponiveis(long obraId)
        {
            var disponiveis = await _service.ObterEquipamentosDisponiveisAsync(obraId);
            return Ok(disponiveis);
        }


        [HttpGet("total-disponiveis")]
        public async Task<IActionResult> GetEquipamentosTotalDisponiveisAsync()
        {
            var disponiveis = await _service.ObterEquipamentosTotalDisponiveisAsync();
            return Ok(disponiveis);
        }

        [HttpGet("total-alocados")]
        public async Task<IActionResult> GetEquipamentosAlocadosAsync()
        {
            var disponiveis = await _service.ObterEquipamentosTotalAlocadosAsync();
            return Ok(disponiveis);
        }
    }

}
