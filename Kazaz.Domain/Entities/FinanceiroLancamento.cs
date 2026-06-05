using System;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public class FinanceiroLancamento : IMultiTenant
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = default!;
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; }
    public StatusLancamento Status { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string Categoria { get; set; } = default!;

    public Guid? ClienteId { get; set; }
    public Pessoa? Cliente { get; set; }

    public Guid? ContratoId { get; set; }
    public Contrato? Contrato { get; set; }

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
