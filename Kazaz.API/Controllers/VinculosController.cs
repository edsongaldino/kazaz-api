using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/vinculos")]
public class VinculosController : ControllerBase
{
    private readonly IVinculoImovelService _service;
    public VinculosController(IVinculoImovelService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Vincular([FromBody] VincularPessoaImovelDto dto, CancellationToken ct)
    {
        await _service.VincularAsync(dto, ct);
        return NoContent();
    }

    // Remove todos os vínculos dessa pessoa com o imóvel,
    // ou apenas o do perfil informado (se perfilId vier).
    [HttpDelete]
    public async Task<IActionResult> Desvincular([FromQuery] Guid pessoaId, [FromQuery] Guid imovelId, [FromQuery] Guid? perfilId, CancellationToken ct)
    {
        await _service.DesvincularAsync(pessoaId, imovelId, perfilId, ct);
        return NoContent();
    }
}