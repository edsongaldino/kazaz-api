using Kazaz.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public sealed class CidadesController : ControllerBase
{
    private readonly ICidadeService _svc;
    public CidadesController(ICidadeService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CidadeDto>>> List([FromQuery] Guid? estadoId, CancellationToken ct)
        => Ok(await _svc.ListAsync(estadoId, ct));

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<CidadeDto>>> Search([FromQuery] string? q, [FromQuery] Guid? estadoId, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, estadoId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CidadeDto>> GetById(Guid id, CancellationToken ct)
    {
        var r = await _svc.GetByIdAsync(id, ct);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("ibge/{ibge}")]
    public async Task<ActionResult<CidadeDto>> GetByIbge(string ibge, CancellationToken ct)
    {
        var r = await _svc.GetByIbgeAsync(ibge, ct);
        return r is null ? NotFound() : Ok(r);
    }
}
