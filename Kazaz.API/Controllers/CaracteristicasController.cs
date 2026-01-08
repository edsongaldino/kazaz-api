using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/caracteristicas")]
public sealed class CaracteristicasController : ControllerBase
{
    private readonly ICaracteristicaService _service;
    public CaracteristicasController(ICaracteristicaService service) => _service = service;

    // GET api/caracteristicas?ativo=true&grupo=Estrutura
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaracteristicaListDto>>> Get(
        [FromQuery] bool? ativo,
        [FromQuery] string? grupo,
        CancellationToken ct)
    {
        var itens = await _service.ListarAsync(ativo, grupo, ct);
        return Ok(itens);
    }
}
