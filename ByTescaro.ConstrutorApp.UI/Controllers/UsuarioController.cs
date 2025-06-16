using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _usuarioService.ObterTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDto dto)
        {
            //var usuario = new Domain.Entities.Admin.Usuario
            //{
            //    Nome = dto.Nome,
            //    Email = dto.Email,
            //    Telefone = dto.Telefone,
            //    Ativo = dto.Ativo,
            //    UsuarioCadastro = "admin",
            //    DataHoraCadastro = DateTime.UtcNow,
            //    PerfilUsuarioId = dto.PerfilUsuarioId
            //};

            //await _usuarioService.CriarAsync(usuario, dto.Senha, "admin");
            await _usuarioService.CriarAsync(dto);
            return Ok(dto.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UsuarioDto dto)
        {
            if (id != dto.Id) return BadRequest("ID do usuário não confere com o DTO"); 

            await _usuarioService.AtualizarAsync(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _usuarioService.ExcluirAsync(id);
            return Ok();
        }

        [HttpPost("inativar/{id}")]
        public async Task<IActionResult> Inativar(long id)
        {
            await _usuarioService.InativarAsync(id, "admin");
            return Ok();
        }
    }
}
