using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/imoveis")]
public class ImoveisController : ControllerBase
{
    private readonly IImovelService _service;
    public ImoveisController(IImovelService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? termo = null, CancellationToken ct = default)
    {
        var (items, total) = await _service.ListarAsync(page, pageSize, termo, ct);
        return Ok(new { page, pageSize, total, items });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await _service.ObterAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ImovelUpsertDto dto, CancellationToken ct)
    {
        var id = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] ImovelUpsertDto dto, CancellationToken ct)
    {
        await _service.AtualizarAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}