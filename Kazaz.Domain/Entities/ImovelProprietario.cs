namespace Kazaz.Domain.Entities;

/// <summary>
/// Vincula um proprietário (Pessoa) diretamente ao Imóvel.
/// O proprietário não muda conforme o contrato; é um atributo permanente do imóvel.
/// </summary>
public class ImovelProprietario
{
    public Guid Id { get; set; }

    public Guid ImovelId { get; set; }
    public Imovel Imovel { get; set; } = null!;

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;

    /// <summary>
    /// Percentual de propriedade (para copropriedade futura). Nulo = proprietário único.
    /// </summary>
    public decimal? Percentual { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
