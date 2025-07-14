// ByTescaro.ConstrutorApp.UI.Controllers/ConfiguracaoLembreteDiarioController.cs
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Authorize] // Pode ser ajustado conforme suas regras de autorização
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracaoLembreteDiarioController : ControllerBase
    {
        private readonly IConfiguracaoLembreteDiarioService _configuracaoLembreteDiarioService;

        public ConfiguracaoLembreteDiarioController(IConfiguracaoLembreteDiarioService configuracaoLembreteDiarioService)
        {
            _configuracaoLembreteDiarioService = configuracaoLembreteDiarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _configuracaoLembreteDiarioService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _configuracaoLembreteDiarioService.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CriarConfiguracaoLembreteDiarioRequest request)
        {
            try
            {
                var createdConfig = await _configuracaoLembreteDiarioService.CreateAsync(request);
                // Retorna 201 Created com a URL para o novo recurso
                return CreatedAtAction(nameof(Get), new { id = createdConfig.Id }, createdConfig);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message }); // Erros de negócio
            }
            catch (Exception ex)
            {
                // Logar a exceção completa aqui
                return StatusCode(500, new { message = "Ocorreu um erro interno ao criar a configuração de lembrete.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] AtualizarConfiguracaoLembreteDiarioRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                await _configuracaoLembreteDiarioService.UpdateAsync(request);
                return NoContent(); // Retorna 204 No Content para atualização bem-sucedida
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message }); // Erros como "não encontrado"
            }
            catch (Exception ex)
            {
                // Logar a exceção completa aqui
                return StatusCode(500, new { message = "Ocorreu um erro interno ao atualizar a configuração de lembrete.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _configuracaoLembreteDiarioService.DeleteAsync(id);
                return NoContent(); // Retorna 204 No Content para exclusão bem-sucedida
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message }); // Erros como "não encontrado"
            }
            catch (Exception ex)
            {
                // Logar a exceção completa aqui
                return StatusCode(500, new { message = "Ocorreu um erro interno ao excluir a configuração de lembrete.", error = ex.Message });
            }
        }
    }
}