using Kazaz.Domain.Entities;

public class Estado
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Uf { get; set; } = null!;     // "MT", "SP", "DF"…

    public ICollection<Cidade> Cidades { get; set; } = new List<Cidade>();
}