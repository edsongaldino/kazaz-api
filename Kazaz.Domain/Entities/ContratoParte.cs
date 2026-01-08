using Kazaz.Domain.Entities;

public class ContratoParte
{
    public Guid Id { get; set; }

    public Guid ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;

    public PapelContrato Papel { get; set; }

    // opcional (deixa pronto para venda com cotas / copropriedade)
    public decimal? Percentual { get; set; }
}
