using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorInsumoController : ControllerBase
    {
        private readonly IFornecedorInsumoService _service;

        public FornecedorInsumoController(IFornecedorInsumoService service)
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

        [HttpGet("por-insumo/{insumoId}")]
        public async Task<IActionResult> GetPorInsumo(long insumoId)
            => Ok(await _service.ObterPorInsumoAsync(insumoId));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FornecedorInsumoDto dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] FornecedorInsumoDto dto)
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
