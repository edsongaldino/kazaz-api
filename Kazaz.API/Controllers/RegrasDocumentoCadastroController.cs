using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/regras-documento-cadastro")]
public class RegrasDocumentoCadastroController : ControllerBase
{
    private readonly IRegrasDocumentoCadastroService _service;

    public RegrasDocumentoCadastroController(IRegrasDocumentoCadastroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RegraDocumentoCadastroResponse>>> Listar(CancellationToken ct)
    {
        var result = await _service.ListarTodasAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarRegraDocumentoCadastroRequest req, CancellationToken ct)
    {
        try
        {
            var id = await _service.CriarAsync(req, ct);
            return CreatedAtAction(nameof(Listar), new { id }, new { id });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarRegraDocumentoCadastroRequest req, CancellationToken ct)
    {
        try
        {
            await _service.AtualizarAsync(id, req, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
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
    }
}
