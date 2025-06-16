using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ObraInsumoController : ControllerBase
{
    private readonly IObraInsumoService _service;

    public ObraInsumoController(IObraInsumoService service)
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
    public async Task<IActionResult> Post([FromBody] ObraInsumoDto dto)
    {
        await _service.CriarAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(long id, [FromBody] ObraInsumoDto dto)
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
    public async Task<IActionResult> GetInsumosDisponiveis(long obraId)
    {
        var disponiveis = await _service.ObterInsumosDisponiveisAsync(obraId);
        return Ok(disponiveis);
    }
}
