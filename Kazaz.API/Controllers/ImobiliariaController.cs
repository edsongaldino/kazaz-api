using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/imobiliaria")]
public class ImobiliariaController(IImobiliariaService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await service.ObterAsync(ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] ImobiliariaUpdateDto dto, CancellationToken ct)
    {
        var result = await service.SalvarAsync(dto, ct);
        return Ok(result);
    }
}
