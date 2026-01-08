using Kazaz.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tipos-imovel")]
public sealed class TiposImovelController : ControllerBase
{
    private readonly ITipoImovelService _service;
    public TiposImovelController(ITipoImovelService service) => _service = service;

    // GET api/tipos-imovel?ativo=true&categoria=URBANO
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TipoImovelListDto>>> Get(
        [FromQuery] bool? ativo,
        [FromQuery] string? categoria,
        CancellationToken ct)
    {
        var itens = await _service.ListarAsync(ativo, categoria, ct);
        return Ok(itens);
    }
}
