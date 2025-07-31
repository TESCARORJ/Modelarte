using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalizacaoController : ControllerBase
    {
        private readonly IPersonalizacaoService _personalizacaoService;

        public PersonalizacaoController(IPersonalizacaoService personalizacaoService)
        {
            _personalizacaoService = personalizacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _personalizacaoService.ObterAsync();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PersonalizacaoDto dto)
        {
            await _personalizacaoService.AtualizarAsync(dto);
            return Ok();
        }
    }
}
