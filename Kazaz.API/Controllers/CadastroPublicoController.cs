using Kazaz.Application.Contracts;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/public/cadastro")]
public class CadastroPublicoController : ControllerBase
{
    private readonly ICadastroPublicoService _service;
    private readonly IContratoConviteService _serviceConvite;
    public CadastroPublicoController(ICadastroPublicoService service, IContratoConviteService serviceConvite)
    {
        _service = service;
        _serviceConvite = serviceConvite;
    }

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

    [HttpGet("{token}/status")]
    public async Task<IActionResult> Status(string token, CancellationToken ct)
    => Ok(await _service.ObterStatusAsync(token, ct));

    [HttpGet("convites-cadastro-contrato")]
    public async Task<ActionResult<PagedResult<ConviteCadastroListItemResponse>>> Listar(
    [FromQuery] ListarConvitesCadastroQuery query,
    CancellationToken ct = default)
    {
        var result = await _serviceConvite.ListarAsync(query, ct);

        return Ok(result);
    }

    [HttpPut("{token}/vincular-pessoa")]
    public async Task<IActionResult> VincularPessoa(
        string token,
        [FromBody] VincularPessoaCadastroPublicoRequest request,
        CancellationToken ct)
    {
        if (request.PessoaId == Guid.Empty)
            return BadRequest("PessoaId inválido.");

        await _service.VincularPessoaAsync(token, request.PessoaId, ct);
        return NoContent();
    }

    [HttpGet("{token}/detalhes")]
    public async Task<ActionResult<CadastroPublicoDetalhesResponse>> ObterDetalhes(
    string token,
    CancellationToken ct)
    {
        var result = await _service.ObterDetalhesAsync(token, ct);
        return Ok(result);
    }

    [HttpPost("{conviteId:guid}/analise")]
    public async Task<IActionResult> AnalisarConvite(
        Guid conviteId,
        [FromBody] AnalisarConviteRequest request,
        CancellationToken ct)
    {
        var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            return Unauthorized("Usuário não identificado.");

        var usuarioId = Guid.Parse(usuarioIdClaim);

        await _serviceConvite.AnalisarConviteAsync(
            conviteId,
            request.Resultado,
            usuarioId,
            request.Comentario,
            ct);

        return NoContent();
    }

}