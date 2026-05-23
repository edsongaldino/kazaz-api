using System;

namespace Kazaz.Domain.Entities;

public class ContratoChecklistSaida
{
    public Guid ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public string? MotivoSaida { get; set; }
    public string? Aluguel { get; set; }
    public string? MultaContratual { get; set; }
    public DateOnly? AvisoSaidaEm { get; set; }
    public string? Chaves { get; set; }
    public string? AvisoProprietario { get; set; }
    public string? Energia { get; set; }
    public string? Gas { get; set; }
    public string? Agua { get; set; }
    public string? Condominio { get; set; }
    public string? Iptu { get; set; }
    public DateOnly? VistoriaSaidaEm { get; set; }
    public string? PinturaManutencao { get; set; }
    public string? ReativarImovelNoSite { get; set; }
    public string? CancelamentoSeguroFianca { get; set; }
    public string? EtapasPersonalizadasJson { get; set; }
}
