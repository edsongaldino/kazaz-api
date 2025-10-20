using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/documentos")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoService _service;
    public DocumentosController(IDocumentoService service) => _service = service;

    [HttpGet("pessoa/{pessoaId:guid}")]
    public async Task<IActionResult> ListarPorPessoa(Guid pessoaId, CancellationToken ct)
    {
        var items = await _service.ListarPorPessoaAsync(pessoaId, ct);
        return Ok(items);
    }

    [HttpGet("imovel/{imovelId:guid}")]
    public async Task<IActionResult> ListarPorImovel(Guid imovelId, CancellationToken ct)
    {
        var items = await _service.ListarPorImovelAsync(imovelId, ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await _service.ObterAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] DocumentoCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] DocumentoUpdateDto dto, CancellationToken ct)
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