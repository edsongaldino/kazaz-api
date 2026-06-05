using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/financeiro")]
public class FinanceiroController(IFinanceiroService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] FinanceiroLancamentoSearchFilterDto filtro, CancellationToken ct)
    {
        var (items, total) = await service.ListarAsync(filtro, ct);
        return Ok(new { items, total });
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> GetResumo(CancellationToken ct)
    {
        var resumo = await service.ObterResumoFinanceiroAsync(ct);
        return Ok(resumo);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var lanc = await service.ObterPorIdAsync(id, ct);
        return Ok(lanc);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FinanceiroLancamentoCreateDto dto, CancellationToken ct)
    {
        var id = await service.CriarAsync(dto, ct);
        var created = await service.ObterPorIdAsync(id, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] FinanceiroLancamentoUpdateDto dto, CancellationToken ct)
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
