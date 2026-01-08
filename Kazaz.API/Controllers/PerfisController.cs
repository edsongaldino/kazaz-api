using Kazaz.Application.DTOs;
using Kazaz.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/perfis")]
public class PerfisController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public PerfisController(ApplicationDbContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PerfilDto>>> Listar(CancellationToken ct)
    {
        var itens = await _ctx.Perfis
            .AsNoTracking()
            .OrderBy(x => x.Nome)
            .Select(x => new PerfilDto(x.Id, x.Nome))
            .ToListAsync(ct);

        return Ok(itens);
    }
}
