using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraRetrabalhoController : ControllerBase
    {
        private readonly IObraRetrabalhoService _service;

        public ObraRetrabalhoController(IObraRetrabalhoService service)
        {
            _service = service;
        }

        [HttpGet("{obraId}")]
        public async Task<IActionResult> GetByObraId(long obraId)
        {
            var list = await _service.ObterPorObraIdAsync(obraId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraRetrabalhoDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraRetrabalhoDto dto)
        {
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
