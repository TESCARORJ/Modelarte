using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _service;

        public ProjetoController(IProjetoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var projetos = await _service.ObterTodosAsync();
            return Ok(projetos);
        }
        
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            var projetos = await _service.ObterTodosListAsync();
            return Ok(projetos);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPorStatus(StatusProjeto status)
        {
            var projetos = await _service.ObterTodosAsync();
            return Ok(projetos.Where(p => p.Status == status));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _service.ObterPorIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjetoDto dto)
        {
            var projetoCriado = await _service.CriarAsync(dto);
            return Ok(projetoCriado); // Retorna o DTO com o Id preenchido
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] ProjetoDto dto)
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

        [HttpGet("projetosAgendados")]
        public async Task<IActionResult> GetAgendados()
        {
            var projetos = await _service.ObterTodosAgendadosAsync();
            return Ok(projetos);
        }

        [HttpGet("projetosEmPlanejamento")]
        public async Task<IActionResult> GetEmPlanejamento()
        {
            var projetos = await _service.ObterTodosEmPlanejamentoAsync();
            return Ok(projetos);
        }

         [HttpGet("projetosConcluidos")]
        public async Task<IActionResult> GetConcluido()
        {
            var projetos = await _service.ObterTodosConcluidosAsync();
            return Ok(projetos);
        }

         [HttpGet("projetosCancelados")]
        public async Task<IActionResult> GetCancelado()
        {
            var projetos = await _service.ObterTodosCanceladosAsync();
            return Ok(projetos);
        }

         [HttpGet("projetosPausados")]
        public async Task<IActionResult> GetPausado()
        {
            var projetos = await _service.ObterTodosPausadosAsync();
            return Ok(projetos);
        }


    }

}