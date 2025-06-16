using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraItemEtapaController : ControllerBase
    {
        private readonly IObraItemEtapaService _service;

        public ObraItemEtapaController(IObraItemEtapaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var etapas = await _service.ObterTodasAsync();
            return Ok(etapas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var etapa = await _service.ObterPorEtapaIdAsync(id);
            return etapa == null ? NotFound() : Ok(etapa);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraItemEtapaDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraItemEtapaDto dto)
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
