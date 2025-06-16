using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraInsumoListaController : ControllerBase
    {
        private readonly IObraInsumoListaService _service;

        public ObraInsumoListaController(IObraInsumoListaService service)
        {
            _service = service;
        }

        [HttpGet("obra/{obraId}")]
        public async Task<IActionResult> GetPorObraId(long obraId)
        {
            var listas = await _service.ObterPorObraIdAsync(obraId);
            return Ok(listas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(long id)
        {
            var lista = await _service.ObterPorIdAsync(id);
            if (lista == null) return NotFound();
            return Ok(lista);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraInsumoListaDto dto)
        {
            var result = await _service.CriarAsync(dto);
            return Ok(result);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraInsumoListaDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _service.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.RemoverAsync(id);
            return Ok();
        }
    }
}
