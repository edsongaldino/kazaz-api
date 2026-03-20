using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.Api.Controllers;

[ApiController]
[Route("api/cadastro-publico")]
public class CadastroPublicoDocumentosController : ControllerBase
{
    private readonly ICadastroPublicoDocumentosService _service;

    public CadastroPublicoDocumentosController(ICadastroPublicoDocumentosService service)
    {
        _service = service;
    }

    // GET /api/cadastro-publico/{token}/documentos-requeridos
    [HttpGet("{token}/documentos-requeridos")]
    public async Task<ActionResult<DocumentosRequeridosResponse>> GetDocumentosRequeridos(
        string token,
        CancellationToken ct)
    {
        var res = await _service.ObterDocumentosRequeridosAsync(token, ct);
        return Ok(res);
    }
}
