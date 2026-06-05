using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImobiliariasController(IImobiliariaService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? termo, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await service.ListarTodasAsync(page, pageSize, termo, ct);
        return Ok(new { items = result.Items, total = result.Total, page, pageSize });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await service.ObterPorIdAsync(id, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ImobiliariaCriarDto dto, CancellationToken ct)
    {
        try
        {
            var result = await service.CriarComAdminAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ImobiliariaUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var result = await service.AtualizarPorIdAsync(id, dto, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await service.ExcluirAsync(id, ct);
        if (!result) return NotFound();
        return NoContent();
    }
}
