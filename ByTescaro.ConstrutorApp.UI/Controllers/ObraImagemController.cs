using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraImagemController : ControllerBase
    {
        private readonly IObraImagemService _service;

        public ObraImagemController(IObraImagemService service)
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
        public async Task<IActionResult> Post([FromBody] ObraImagemDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok(dto); // Retorna o DTO com o ID e outros campos atualizados pelo serviço
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.RemoverAsync(id);
            return Ok();
        }
    }
}
