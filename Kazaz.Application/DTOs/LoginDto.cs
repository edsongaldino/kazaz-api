namespace Kazaz.Application.DTOs;

public class LoginDto
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

public class LoginResponseDto
{
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public Guid? PerfilId { get; set; }
    public string? PerfilNome { get; set; }
}