using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/imoveis/{imovelId:guid}/fotos")]
public class FotosController : ControllerBase
{
    private readonly IFotoService _service;
    public FotosController(IFotoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar(Guid imovelId, CancellationToken ct)
    {
        var items = await _service.ListarPorImovelAsync(imovelId, ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid imovelId, Guid id, CancellationToken ct)
    {
        var dto = await _service.ObterAsync(id, ct);
        if (dto is null || dto.ImovelId != imovelId) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(Guid imovelId, [FromBody] FotoCreateDto dto, CancellationToken ct)
    {
        if (dto.ImovelId != imovelId) return BadRequest("ImovelId do corpo difere do da rota.");
        var id = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { imovelId, id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid imovelId, Guid id, [FromBody] FotoUpdateDto dto, CancellationToken ct)
    {
        await _service.AtualizarAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid imovelId, Guid id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }

    public record ReordemItem(Guid FotoId, int Ordem);

    [HttpPost("reordenar")]
    public async Task<IActionResult> Reordenar(Guid imovelId, [FromBody] List<ReordemItem> itens, CancellationToken ct)
    {
        await _service.ReordenarAsync(imovelId, itens.Select(x => (x.FotoId, x.Ordem)), ct);
        return NoContent();
    }
}