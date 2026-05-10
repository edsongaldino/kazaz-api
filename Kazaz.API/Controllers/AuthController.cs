using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kazaz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _service;

    public AuthController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var response = await _service.AutenticarAsync(login);

        if (response is null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        return Ok(response);
    }
}