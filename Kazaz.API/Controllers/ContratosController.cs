using Kazaz.Application.Contracts;
using Kazaz.Application.DTOs;
using Kazaz.Application.Services;
using Kazaz.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.Api.Controllers;

[ApiController]
[Route("api/contratos")]
public class ContratosController : ControllerBase
{
    private readonly IContratosService _service;
    private readonly IContratoConviteService _contratoConviteService;

    public ContratosController(IContratoConviteService contratoConviteService, IContratosService service)
    {
        _service = service;
        _contratoConviteService = contratoConviteService;
    }

    [HttpPost]
    public async Task<ActionResult<ContratoResponse>> Criar([FromBody] CriarContratoRequest req, CancellationToken ct)
    {
        try
        {
            var res = await _service.CriarRascunhoAsync(req, ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = res.Id }, res);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/ativar")]
    public async Task<ActionResult<ContratoResponse>> Ativar(Guid id, CancellationToken ct)
    {
        try
        {
            var res = await _service.AtivarAsync(id, ct);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<ContratoResponse>> Cancelar(Guid id, CancellationToken ct)
    {
        try
        {
            var res = await _service.CancelarAsync(id, ct);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/encerrar")]
    public async Task<ActionResult<ContratoResponse>> Encerrar(Guid id, CancellationToken ct)
    {
        try
        {
            var res = await _service.EncerrarAsync(id, ct);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContratoResponse>> ObterPorId(Guid id, CancellationToken ct)
    {
        try
        {
            var res = await _service.ObterPorIdAsync(id, ct);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ContratoResponse>>> Listar(
        [FromQuery] Guid? imovelId,
        [FromQuery] TipoContrato? tipo,
        [FromQuery] StatusContrato? status,
        CancellationToken ct)
    {
        var res = await _service.ListarAsync(imovelId, tipo, status, ct);
        return Ok(res);
    }



    [HttpPost("rascunho/gerar-links")]
    public async Task<ActionResult<GerarLinksContratoResponse>> GerarLinks(
        Guid imovelId,
        GerarLinksContratoRequest request,
        CancellationToken ct)
    {
        var result = await _contratoConviteService.GerarLinksAsync(imovelId, request, ct);
        return Ok(result);
    }
}
