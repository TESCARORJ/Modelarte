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
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarioService;

        public FuncionarioController(IFuncionarioService funcionarioService)
        {
            _funcionarioService = funcionarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _funcionarioService.ObterTodosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _funcionarioService.ObterPorIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FuncionarioDto dto)
        {
            await _funcionarioService.CriarAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] FuncionarioDto dto)
        {
            if (id != dto.Id) return BadRequest();
            await _funcionarioService.AtualizarAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _funcionarioService.RemoverAsync(id);
            return Ok();
        }

        //[HttpGet("ObterResumoAlocacaoAsync")]
        //public async Task<IActionResult> ObterResumoAlocacaoAsync()
        //{
        //    var (alocados, naoAlocados) = await _funcionarioService.ObterResumoAlocacaoAsync();

        //    var dto = new ResumoAlocacaoDto
        //    {
        //        Alocados = alocados,
        //        NaoAlocados = naoAlocados
        //    };

        //    return Ok(dto);
        //}

        [HttpGet("CpfCnpjExists")]
        public async Task<IActionResult> CpfCnpjExists(string cpfCnpj, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
            {
                return BadRequest("CPF/CNPJ não pode ser vazio.");
            }

            //string cleanedCpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());

            // A consulta real dependerá do seu repositório ou serviço de domínio.
            // Aqui, apenas um exemplo lógico.
            bool exists = await _funcionarioService.CpfCnpjExistsAsync(cpfCnpj);

            return Ok(exists);
        }

        [HttpGet("TelefonePrincipalExists")]
        public async Task<IActionResult> TelefonePrincipalExists(string telefonePrincipal, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefonePrincipal)) return BadRequest("Telefone Principal não pode ser vazio.");
            bool exists = await _funcionarioService.TelefonePrincipalExistsAsync(telefonePrincipal, ignoreId);
            return Ok(exists);
        }

        [HttpGet("TelefoneWhatsAppExists")]
        public async Task<IActionResult> TelefoneWhatsAppExists(string telefoneWhatsApp, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefoneWhatsApp)) return BadRequest("WhatsApp não pode ser vazio.");
            bool exists = await _funcionarioService.TelefoneWhatsAppExistsAsync(telefoneWhatsApp, ignoreId);
            return Ok(exists);
        }

        [HttpGet("EmailExists")]
        public async Task<IActionResult> EmailExists(string email, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email não pode ser vazio.");
            bool exists = await _funcionarioService.EmailExistsAsync(email, ignoreId);
            return Ok(exists);
        }
    }

}