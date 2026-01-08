using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/public/cadastro")]
public class CadastroPublicoController : ControllerBase
{
    private readonly ICadastroPublicoService _service;
    public CadastroPublicoController(ICadastroPublicoService service) => _service = service;

    [HttpGet("{token}")]
    public async Task<IActionResult> ObterConvite(string token, CancellationToken ct)
        => Ok(await _service.ObterConviteAsync(token, ct));

    // 👇 Reaproveita seu DTO: "FinalizarCadastroPublicoRequest"
    // mas aqui é o passo 1 (iniciar): manda Nome/EnderecoId e Documentos vazio.
    [HttpPost("{token}/iniciar")]
    public async Task<IActionResult> Iniciar(string token, [FromBody] FinalizarCadastroPublicoRequest request, CancellationToken ct)
        => Ok(await _service.IniciarAsync(token, request, ct));

    [HttpPost("{token}/concluir")]
    public async Task<IActionResult> Concluir(string token, CancellationToken ct)
        => Ok(await _service.ConcluirAsync(token, ct));
}