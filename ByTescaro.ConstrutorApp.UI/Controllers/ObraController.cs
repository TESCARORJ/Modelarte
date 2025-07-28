using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObraController : ControllerBase
    {
        private readonly IObraService _service;
        private readonly ILogger<ObraController> _logger;

        public ObraController(IObraService service, ILogger<ObraController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var obras = await _service.ObterTodosAsync();
                return Ok(obras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as obras.");
                // Retorna um 500 Internal Server Error com uma mensagem genérica.
                // Os detalhes do erro estarão no log do servidor.
                return StatusCode(500, "Ocorreu um erro interno no servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var obra = await _service.ObterPorIdAsync(id);
            return obra == null ? NotFound() : Ok(obra);
        }

        [HttpGet("por-projeto/{projetoId}")]
        public async Task<ActionResult<List<ObraDto>>> GetByProjetoId(long projetoId)
        {
            var obras = await _service.ObterPorProjetoAsync(projetoId);
            return Ok(obras);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObraDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dtoCriado = await _service.CriarAsync(dto);

            // Retorna um status 201 Created com o DTO no corpo e um link para o novo recurso no cabeçalho Location
            return CreatedAtAction(nameof(Get), new { id = dtoCriado.Id }, dtoCriado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ObraDto dto)
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

        [HttpGet("{id}/progresso")]
        public async Task<IActionResult> GetProgresso(long id)
        {
            var progresso = await _service.CalcularProgressoAsync(id);
            return Ok(progresso);
        }

        [HttpGet("{id}/etapas")]
        public async Task<IActionResult> GetEtapas(long id)
        {
            var etapas = await _service.ObterEtapasDaObraAsync(id);
            return Ok(etapas);
        }

        [HttpGet("itensEtapas/{obraEtapaId}")]
        public async Task<IActionResult> GetItensEtapas(long obraEtapaId)
        {
            var itens = await _service.ObterItensDaEtapaAsync(obraEtapaId);
            return Ok(itens);
        }

        [HttpPatch("concluirItem/{itemId}")]
        public async Task<IActionResult> AtualizarConclusaoItem(long itemId, [FromBody] bool concluido)
        {
            await _service.AtualizarConclusaoItemAsync(itemId, concluido);
            return Ok();
        }
    }

}