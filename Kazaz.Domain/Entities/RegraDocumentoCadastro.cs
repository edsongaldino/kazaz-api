namespace Kazaz.Domain.Entities;

public enum TipoPessoaRule
{
    Any = 0,
    PF = 1,
    PJ = 2
}

public enum TipoContratoRule
{
    Any = 0,
    Locacao = 1,
    Venda = 2,
    Compra = 3
}

public enum PapelContratoRule
{
    Any = 0,
    Locador = 1,
    Locatario = 2,
    Fiador = 3,
    Vendedor = 10,
    Comprador = 11
}

public class RegraDocumentoCadastro
{
    public Guid Id { get; set; }

    public TipoPessoaRule TipoPessoa { get; set; } = TipoPessoaRule.Any;
    public TipoContratoRule TipoContrato { get; set; } = TipoContratoRule.Any;
    public PapelContratoRule PapelContrato { get; set; } = PapelContratoRule.Any;

    public Guid TipoDocumentoId { get; set; }
    public TipoDocumento TipoDocumento { get; set; } = default!;

    public bool Obrigatorio { get; set; } = true;

    public int Ordem { get; set; } = 0;

    /// <summary>
    /// Quantas instâncias deste documento devem aparecer (ex.: Sócio Adm. (1..N)).
    /// Default 1.
    /// </summary>
    public int Multiplicidade { get; set; } = 1;

    /// <summary>
    /// Se informado, sobrescreve o nome do TipoDocumento na lista.
    /// Pode conter "(1)" etc. quando Multiplicidade=1.
    /// </summary>
    public string? Rotulo { get; set; }

    public bool Ativo { get; set; } = true;
}
