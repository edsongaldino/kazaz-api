namespace Kazaz.Application.DTOs;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public bool Ativo { get; set; }
    public Guid PerfilId { get; set; }
}

public class UsuarioListDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool Ativo { get; set; }
    public Guid PerfilId { get; set; }
    public string? PerfilNome { get; set; } // opcional (recomendado)
}

public class UsuarioCreateDto
{
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public bool Ativo { get; set; } = true;
    public Guid PerfilId { get; set; }
}
public class UsuarioUpdateDto
{
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool Ativo { get; set; }
    public Guid PerfilId { get; set; }
}

public record UsuarioResetSenhaDto(string NovaSenha);
