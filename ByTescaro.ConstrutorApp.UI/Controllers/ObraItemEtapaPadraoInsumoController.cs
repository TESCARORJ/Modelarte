using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraItemEtapaPadraoInsumoController : ControllerBase
    {
        private readonly IObraItemEtapaPadraoInsumoService _service;

        public ObraItemEtapaPadraoInsumoController(IObraItemEtapaPadraoInsumoService service)
        {
            _service = service;
        }

        [HttpGet("{itemPadraoId}")]
        public async Task<IActionResult> GetByItemPadraoId(long itemPadraoId)
        {
            var lista = await _service.ObterPorItemPadraoIdAsync(itemPadraoId);
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraItemEtapaPadraoInsumoDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraItemEtapaPadraoInsumoDto dto)
        {
            dto.Id = id;
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
