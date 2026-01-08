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

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<ContratoParte> Partes { get; set; } = new List<ContratoParte>();
}
