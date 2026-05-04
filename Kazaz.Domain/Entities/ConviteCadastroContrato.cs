namespace Kazaz.Domain.Entities;

public enum StatusConviteCadastro
{
    PendentePreenchimento = 1,
    Preenchido = 2,
    EmAnalise = 3,
    Aprovado = 4,
    Reprovado = 5,
    CorrecaoSolicitada = 6,
    Expirado = 7,
    Cancelado = 8
}

public class ConviteCadastroContrato
{
    public Guid Id { get; set; }

    public Guid ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public PapelContrato Papel { get; set; }

    public string Token { get; set; } = null!;

    public StatusConviteCadastro Status { get; set; } =
        StatusConviteCadastro.PendentePreenchimento;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiraEm { get; set; }

    public Guid? PessoaId { get; set; }
    public Pessoa? Pessoa { get; set; }

    public DateTime? PreenchidoEm { get; set; }
    public DateTime? UsadoEm { get; set; }

    public ICollection<AnaliseConvite> Analises { get; set; } = new List<AnaliseConvite>();
}
