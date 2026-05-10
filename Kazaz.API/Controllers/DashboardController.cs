using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("resumo")]
    public async Task<IActionResult> ObterResumo(CancellationToken ct)
    {
        var resumo = await _service.ObterResumoAsync(ct);
        return Ok(resumo);
    }
}