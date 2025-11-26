namespace Kazaz.Domain.Entities;

public sealed class Conjuge
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public DateOnly? DataNascimento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
}