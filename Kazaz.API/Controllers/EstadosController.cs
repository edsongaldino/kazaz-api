using Microsoft.AspNetCore.Mvc;
using Kazaz.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public sealed class EstadosController : ControllerBase
{
    private readonly IEstadoService _svc;
    public EstadosController(IEstadoService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoDto>>> List(CancellationToken ct)
        => Ok(await _svc.ListAsync(ct));

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<EstadoDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpGet("uf/{uf}")]
    public async Task<ActionResult<EstadoDto>> GetByUf(string uf, CancellationToken ct)
    {
        var r = await _svc.GetByUfAsync(uf, ct);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EstadoDto>> GetById(Guid id, CancellationToken ct)
    {
        var r = await _svc.GetByIdAsync(id, ct);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("{estadoId:guid}/cidades")]
    public async Task<ActionResult<IEnumerable<CidadeSlimDto>>> ListCidades(Guid estadoId, CancellationToken ct)
        => Ok(await _svc.ListCidadesAsync(estadoId, ct));

}
