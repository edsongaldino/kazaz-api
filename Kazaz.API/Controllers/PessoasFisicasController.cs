using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/pessoas/fisicas")]
public class PessoasFisicasController : ControllerBase
{
    private readonly IPessoaFisicaService _service;
    public PessoasFisicasController(IPessoaFisicaService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PessoaFisicaCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CriarAsync(dto, ct);
        return Created($"/api/pessoas/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PessoaFisicaUpdateDto dto, CancellationToken ct)
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
