using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [ApiController]
    [Route("api/obras/{obraId}/checklist")]
    public class ObraChecklistController : ControllerBase
    {
        private readonly IObraChecklistService _service;

        public ObraChecklistController(IObraChecklistService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ObraEtapaDto>>> Get(long obraId)
            => Ok(await _service.ObterChecklistAsync(obraId));

        [HttpPut]
        public async Task<IActionResult> Put(long obraId, List<ObraEtapaDto> etapas)
        {
            await _service.SalvarChecklistAsync(obraId, etapas);
            return NoContent();
        }
    }

}
