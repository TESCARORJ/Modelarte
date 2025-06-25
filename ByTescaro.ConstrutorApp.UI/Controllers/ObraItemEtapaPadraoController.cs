using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraItemEtapaPadraoController : ControllerBase
    {
        private readonly IObraItemEtapaPadraoService _service;

        public ObraItemEtapaPadraoController(IObraItemEtapaPadraoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var itens = await _service.ObterTodasAsync();
            return Ok(itens);
        }

        [HttpGet("por-etapa/{etapaPadraoId}")]
        public async Task<IActionResult> GetByEtapaPadrao(long etapaPadraoId)
        {
            var itens = await _service.ObterPorEtapaIdAsync(etapaPadraoId);
            return Ok(itens);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _service.ObterPorIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraItemEtapaPadraoDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var createdDto = await _service.CriarAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
            }
            catch (DuplicateRecordException ex)
            {
                // Retorna um status 409 Conflict com a mensagem amigável no corpo
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Para qualquer outro erro inesperado, retorna um 500
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor.", details = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraItemEtapaPadraoDto dto)
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
    }

}