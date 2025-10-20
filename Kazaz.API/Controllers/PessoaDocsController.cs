using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/pessoas/{pessoaId:guid}/documentos")]
public class PessoaDocsController : ControllerBase
{
    private readonly IAnexoDocumentoService _anexo;
    private readonly IValidacaoDocumentoService _val;
    public PessoaDocsController(IAnexoDocumentoService a, IValidacaoDocumentoService v) { _anexo = a; _val = v; }

    [HttpPost]
    public async Task<IActionResult> Anexar(Guid pessoaId, [FromBody] AnexarDocumentoPessoaDto dto, CancellationToken ct)
    {
        if (dto.PessoaId != pessoaId) return BadRequest("PessoaId divergente.");
        var id = await _anexo.AnexarParaPessoaAsync(dto, ct);
        return Created($"/api/documentos/{id}", new { id });
    }

    [HttpDelete("{tipoDocumentoId:guid}")]
    public async Task<IActionResult> Remover(Guid pessoaId, Guid tipoDocumentoId, CancellationToken ct)
    {
        await _anexo.RemoverPessoaAsync(pessoaId, tipoDocumentoId, ct);
        return NoContent();
    }

    [HttpGet("faltantes")]
    public async Task<IActionResult> Faltantes(Guid pessoaId, CancellationToken ct)
        => Ok(await _val.FaltantesPessoaAsync(pessoaId, ct));
}
