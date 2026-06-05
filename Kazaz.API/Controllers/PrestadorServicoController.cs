using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/prestadores")]
public class PrestadorServicoController(IPrestadorServicoService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PrestadorServicoSearchFilterDto filtro, CancellationToken ct)
    {
        var (items, total) = await service.ListarAsync(filtro, ct);
        return Ok(new { items, total });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var prest = await service.ObterPorIdAsync(id, ct);
        return Ok(prest);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PrestadorServicoCreateDto dto, CancellationToken ct)
    {
        var id = await service.CriarAsync(dto, ct);
        var created = await service.ObterPorIdAsync(id, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] PrestadorServicoUpdateDto dto, CancellationToken ct)
    {
        await service.AtualizarAsync(id, dto, ct);
        var updated = await service.ObterPorIdAsync(id, ct);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.RemoverAsync(id, ct);
        return NoContent();
    }
}
