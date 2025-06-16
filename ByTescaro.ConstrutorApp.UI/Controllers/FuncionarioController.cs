using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarioService;

        public FuncionarioController(IFuncionarioService funcionarioService)
        {
            _funcionarioService = funcionarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _funcionarioService.ObterTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _funcionarioService.ObterPorIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FuncionarioDto dto)
        {
            await _funcionarioService.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] FuncionarioDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _funcionarioService.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _funcionarioService.RemoverAsync(id);
            return Ok();
        }

        [HttpGet("ObterResumoAlocacaoAsync")]
        public async Task<IActionResult> ObterResumoAlocacaoAsync()
        {
            var (alocados, naoAlocados) = await _funcionarioService.ObterResumoAlocacaoAsync();

            var dto = new ResumoAlocacaoDto
            {
                Alocados = alocados,
                NaoAlocados = naoAlocados
            };

            return Ok(dto);
        }
    }

}