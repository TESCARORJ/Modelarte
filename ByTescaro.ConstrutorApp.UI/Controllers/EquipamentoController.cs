using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EquipamentoController : ControllerBase
    {
        private readonly IEquipamentoService _service;

        public EquipamentoController(IEquipamentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _service.ObterTodosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _service.ObterPorIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EquipamentoDto dto)
        {
            await _service.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] EquipamentoDto dto)
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

        //[HttpGet("ObterResumoAlocacaoAsync")]
        //public async Task<IActionResult> ObterResumoAlocacaoAsync()
        //{
        //    var (alocados, naoAlocados) = await _service.ObterResumoAlocacaoAsync();

        //    var dto = new ResumoAlocacaoDto
        //    {
        //        Alocados = alocados,
        //        NaoAlocados = naoAlocados
        //    };

        //    return Ok(dto);
        //}

        [HttpGet("NomeExists")]
        public async Task<IActionResult> NomeExists(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest("O nome do equipamento não pode ser vazio.");
            }
            bool exists = await _service.NomeExistsAsync(nome, ignoreId);
            return Ok(exists);
        }

        [HttpGet("PatrimonioExists")]
        public async Task<IActionResult> PatrimonioExists(string patrimonio, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(patrimonio))
            {
                return BadRequest("O patrimônio do equipamento não pode ser vazio.");
            }
            bool exists = await _service.PatrimonioExistsAsync(patrimonio, ignoreId);
            return Ok(exists);
        }
    }

}