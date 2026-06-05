using System;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public class PrestadorServico : IMultiTenant
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public EspecialidadePrestador Especialidade { get; set; }
    public string CpfCnpj { get; set; } = default!;
    public string Telefone { get; set; } = default!;
    public string? Email { get; set; }
    public bool Ativo { get; set; } = true;
    public string? Observacoes { get; set; }

    public Guid? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
