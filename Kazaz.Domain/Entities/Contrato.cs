namespace Kazaz.Domain.Entities;

public class Contrato
{
    public Guid Id { get; set; }

    public TipoContrato Tipo { get; set; }
    public StatusContrato Status { get; set; } = StatusContrato.Rascunho;

    public Guid ImovelId { get; set; }
    public Imovel Imovel { get; set; } = null!;

    public DateOnly InicioVigencia { get; set; }
    public DateOnly? FimVigencia { get; set; }

    public string Numero { get; set; } = null!; // unique, ex: 2026-000123

    /// <summary>
    /// Forma de garantia do locatrio (apenas para Locação).
    /// Define se o contrato tem Fiador ou Seguro Fiança.
    /// </summary>
    public FormaGarantiaLocacao? FormaGarantia { get; set; }

    /// <summary>
    /// Indica se este contrato será administrado diretamente pelo proprietário
    /// (em vez da imobiliária). Caso especial e pouco comum.
    /// </summary>
    public bool AdministradoPeloProprietario { get; set; } = false;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<ContratoParte> Partes { get; set; } = new List<ContratoParte>();
}
