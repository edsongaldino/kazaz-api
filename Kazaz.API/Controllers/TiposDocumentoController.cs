using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

    [HttpGet("todos")]
    public async Task<IActionResult> ListarTodos(CancellationToken ct)
        => Ok(await _service.ListarTodosAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoDocumentoCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(ListarTodos), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] TipoDocumentoUpdateDto dto, CancellationToken ct)
    {
        try
        {
            await _service.AtualizarAsync(id, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.ExcluirAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
