using System;

namespace Kazaz.Domain.Entities;

public class ContratoChecklistEntrada
{
    public Guid ContratoId { get; set; }
    public Contrato Contrato { get; set; } = null!;

    public DateOnly? AssinadoEm { get; set; }
    public string? SeguroIncendio { get; set; }
    public string? Chaves { get; set; }
    public string? Energia { get; set; }
    public string? Agua { get; set; }
    public string? Gas { get; set; }
    public string? Condominio { get; set; }
    public string? IptuGaragem { get; set; }
    public string? Iptu { get; set; }
    public DateOnly? VistoriaEntradaEm { get; set; }
    public string? Manutencao { get; set; }
    public string? ObservacoesFinais { get; set; }
    public string? BonusLocacao { get; set; }
    public DateOnly? DataPagamentoBonus { get; set; }
    public string? EtapasPersonalizadasJson { get; set; }
}
