using System;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public class Lead : IMultiTenant
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public Guid? OrigemId { get; set; }
    public Origem? Origem { get; set; }
    public Guid? ImovelId { get; set; }
    public Imovel? Imovel { get; set; }
    public LeadStatus Status { get; set; }
    public string? Mensagem { get; set; }
    public Guid? PessoaId { get; set; }
    public Pessoa? Pessoa { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
