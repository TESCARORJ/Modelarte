using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ObraServicoController : ControllerBase
{
    private readonly IObraServicoService _service;

    public ObraServicoController(IObraServicoService service)
    {
        _service = service;
    }

    [HttpGet("lista/{listaId}")]
    public async Task<IActionResult> GetByListaId(long listaId)
    {
        var lista = await _service.ObterPorListaIdAsync(listaId);
        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ObraServicoDto dto)
    {
        await _service.CriarAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] ObraServicoDto dto)
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

    [HttpGet("disponiveis/{obraId}")]
    public async Task<IActionResult> GetServicosDisponiveis(long obraId)
    {
        var disponiveis = await _service.ObterServicosDisponiveisAsync(obraId);
        return Ok(disponiveis);
    }
}
