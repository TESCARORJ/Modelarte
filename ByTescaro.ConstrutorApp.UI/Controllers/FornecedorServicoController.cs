using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorServicoController : ControllerBase
    {
        private readonly IFornecedorServicoService _service;

        public FornecedorServicoController(IFornecedorServicoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var item = await _service.ObterPorIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("por-fornecedor/{fornecedorId}")]
        public async Task<IActionResult> GetPorFornecedor(long fornecedorId)
            => Ok(await _service.ObterPorFornecedorAsync(fornecedorId));

        [HttpGet("por-servico/{servicoId}")]
        public async Task<IActionResult> GetPorServico(long servicoId)
            => Ok(await _service.ObterPorServicoAsync(servicoId));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FornecedorServicoDto dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] FornecedorServicoDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _service.AtualizarAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }

}
