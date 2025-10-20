using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/tipos-documento")]
public class TiposDocumentoController : ControllerBase
{
    private readonly ITipoDocumentoService _service;
    public TiposDocumentoController(ITipoDocumentoService s) => _service = s;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] AlvoDocumento alvo, [FromQuery] bool? obrigatorios, CancellationToken ct)
        => Ok(await _service.ListarPorAlvoAsync(alvo, obrigatorios, ct));
}
