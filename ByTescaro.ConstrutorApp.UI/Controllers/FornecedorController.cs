using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorController : ControllerBase
    {
        private readonly IFornecedorService _FornecedorService;

        public FornecedorController(IFornecedorService FornecedorService)
        {
            _FornecedorService = FornecedorService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _FornecedorService.ObterTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _FornecedorService.ObterPorIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FornecedorDto dto)
        {
            await _FornecedorService.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] FornecedorDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _FornecedorService.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _FornecedorService.RemoverAsync(id);
            return Ok();
        }
    }

}