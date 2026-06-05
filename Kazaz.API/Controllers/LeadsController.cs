using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController(ILeadService service) : ControllerBase
{
    /// <summary>Lista paginada de leads</summary>
    [HttpGet]
    [ProducesResponseType(typeof(LeadsPagedResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? nome, [FromQuery] string? email, [FromQuery] string? telefone, [FromQuery] LeadStatus? status, [FromQuery] Guid? origemId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var filter = new LeadSearchFilterDto(nome, email, telefone, status, origemId, page, pageSize);
        var result = await service.SearchAsync(filter, ct);
        return Ok(result);
    }

    /// <summary>Obtém um lead pelo Id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var dto = await service.GetByIdAsync(id, ct);
            return Ok(dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Cria um lead</summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] LeadCreateDto dto, CancellationToken ct)
    {
        try
        {
            var id = await service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Atualiza um lead</summary>
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LeadUpdateDto dto, CancellationToken ct)
    {
        try
        {
            await service.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Exclui um lead</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Converte um lead em cliente</summary>
    [HttpPost("{id:guid}/convert")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConvertToClient(Guid id, [FromBody] ConvertLeadRequest request, CancellationToken ct)
    {
        try
        {
            var pessoaId = await service.ConvertToClientAsync(id, request, ct);
            return Ok(new { pessoaId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
