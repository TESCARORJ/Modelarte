using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilUsuarioController : ControllerBase
    {
        private readonly IPerfilUsuarioService _perfilUsuarioService;

        public PerfilUsuarioController(IPerfilUsuarioService perfilUsuarioService)
        {
            _perfilUsuarioService = perfilUsuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _perfilUsuarioService.ObterTodosAsync();
            return Ok(result);
        }

        //[HttpGet("disponiveis")]
        //public async Task<IActionResult> GetDisponiveis()
        //{
        //    var result = await _perfilUsuarioService.ObterTodosAsync();
        //    return Ok(result);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _perfilUsuarioService.ObterPorIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PerfilUsuarioDto dto)
        {
            await _perfilUsuarioService.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] PerfilUsuarioDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _perfilUsuarioService.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _perfilUsuarioService.RemoverAsync(id);
            return Ok();
        }
    }

}