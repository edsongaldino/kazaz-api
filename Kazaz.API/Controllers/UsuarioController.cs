using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuarioController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.ObterTodosAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var usuario = await _service.ObterPorIdAsync(id);
        return usuario == null ? NotFound() : Ok(usuario);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioDto dto)
    {
        var novo = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UsuarioDto dto)
    {
        var atualizado = await _service.AtualizarAsync(id, dto);
        return atualizado == null ? NotFound() : Ok(atualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sucesso = await _service.RemoverAsync(id);
        return sucesso ? NoContent() : NotFound();
    }
}