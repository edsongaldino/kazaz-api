namespace Kazaz.Domain.Entities;

public class Cidade
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Ibge { get; set; } = null!;   // 7 dígitos, ex: "5103403"

    public Guid EstadoId { get; set; }
    public Estado Estado { get; set; } = null!;
}
