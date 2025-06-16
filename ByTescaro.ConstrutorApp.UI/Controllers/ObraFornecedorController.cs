using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraFornecedorController : ControllerBase
    {
        private readonly IObraFornecedorService _service;

        public ObraFornecedorController(IObraFornecedorService service)
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
        public async Task<IActionResult> Post([FromBody] ObraFornecedorDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpDelete("{fornecedorId}")]
        public async Task<IActionResult> Delete(long fornecedorId)
        {
            await _service.RemoverAsync( fornecedorId);
            return Ok();
        }

        [HttpGet("disponiveis/{obraId}")]
        public async Task<IActionResult> GetFornecedoresDisponiveis(long obraId)
        {
            var disponiveis = await _service.ObterFornecedoresDisponiveisAsync(obraId);
            return Ok(disponiveis);
        }


        [HttpGet("total-disponiveis")]
        public async Task<IActionResult> GetFornecedoresTotalDisponiveisAsync()
        {
            var disponiveis = await _service.ObterFornecedoresTotalDisponiveisAsync();
            return Ok(disponiveis);
        }

        [HttpGet("total-alocados")]
        public async Task<IActionResult> GetFornecedoresAlocadosAsync()
        {
            var disponiveis = await _service.ObterFornecedoresTotalAlocadosAsync();
            return Ok(disponiveis);
        }
    }

}
