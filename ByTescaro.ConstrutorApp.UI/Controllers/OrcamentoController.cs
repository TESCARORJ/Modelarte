using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrcamentoController : ControllerBase
    {
        private readonly IOrcamentoService _orcamentoService;

        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            _orcamentoService = orcamentoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orcamentos = await _orcamentoService.ObterTodosAsync();
            return Ok(orcamentos);
        }


        [HttpGet("obra/{obraId}")]
        public async Task<IActionResult> GetByObra(long obraId)
        {
            var result = await _orcamentoService.ObterPorObraAsync(obraId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _orcamentoService.ObterPorIdComItensAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrcamentoDto dto)
        {
            await _orcamentoService.CriarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _orcamentoService.RemoverAsync(id);
            return NoContent();
        }

    }
}
