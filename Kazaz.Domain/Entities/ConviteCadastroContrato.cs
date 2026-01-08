namespace Kazaz.Domain.Entities;

public enum StatusConviteCadastro
{
    Pendente = 1,
    Usado = 2,
    Expirado = 3,
    Cancelado = 4
}

public class ConviteCadastroContrato
{
    public Guid Id { get; set; }

    public Guid ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public PapelContrato Papel { get; set; }

    public string Token { get; set; } = null!; // unique

    public StatusConviteCadastro Status { get; set; } = StatusConviteCadastro.Pendente;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiraEm { get; set; }

    public Guid? PessoaId { get; set; }
    public Pessoa? Pessoa { get; set; }

    public DateTime? UsadoEm { get; set; }
}
