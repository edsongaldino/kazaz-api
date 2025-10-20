using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/imoveis/{imovelId:guid}/documentos")]
public class ImovelDocsController : ControllerBase
{
    private readonly IAnexoDocumentoService _anexo;
    private readonly IValidacaoDocumentoService _val;
    public ImovelDocsController(IAnexoDocumentoService a, IValidacaoDocumentoService v) { _anexo = a; _val = v; }

    [HttpPost]
    public async Task<IActionResult> Anexar(Guid imovelId, [FromBody] AnexarDocumentoImovelDto dto, CancellationToken ct)
    {
        if (dto.ImovelId != imovelId) return BadRequest("ImovelId divergente.");
        var id = await _anexo.AnexarParaImovelAsync(dto, ct);
        return Created($"/api/documentos/{id}", new { id });
    }

    [HttpDelete("{tipoDocumentoId:guid}")]
    public async Task<IActionResult> Remover(Guid imovelId, Guid tipoDocumentoId, CancellationToken ct)
    {
        await _anexo.RemoverImovelAsync(imovelId, tipoDocumentoId, ct);
        return NoContent();
    }

    [HttpGet("faltantes")]
    public async Task<IActionResult> Faltantes(Guid imovelId, CancellationToken ct)
        => Ok(await _val.FaltantesImovelAsync(imovelId, ct));
}
