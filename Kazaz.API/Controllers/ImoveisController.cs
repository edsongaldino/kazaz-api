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
    public async Task<IActionResult> Listar(
    [FromQuery] ListarImoveisQuery query,
    CancellationToken ct = default)
    {
        var (items, total) = await _service.ListarAsync(query, ct);

        return Ok(new
        {
            page = query.Page,
            pageSize = query.PageSize,
            total,
            items
        });
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

    // ---- Proprietarios ----

    [HttpGet("{id:guid}/proprietarios")]
    public async Task<IActionResult> ListarProprietarios(Guid id, CancellationToken ct)
    {
        var lista = await _service.ListarProprietariosAsync(id, ct);
        return Ok(lista);
    }

    [HttpPost("{id:guid}/proprietarios")]
    public async Task<IActionResult> AdicionarProprietario(
        Guid id,
        [FromBody] AdicionarProprietarioRequest req,
        CancellationToken ct)
    {
        try
        {
            var dto = await _service.AdicionarProprietarioAsync(id, req, ct);
            return Created(string.Empty, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/proprietarios/{proprietarioId:guid}")]
    public async Task<IActionResult> RemoverProprietario(Guid id, Guid proprietarioId, CancellationToken ct)
    {
        try
        {
            await _service.RemoverProprietarioAsync(id, proprietarioId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}