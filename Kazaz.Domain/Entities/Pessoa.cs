using System;
using System.Collections.Generic;
using Kazaz.Domain.Interfaces;

namespace Kazaz.Domain.Entities;

public class Pessoa : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid? EnderecoId { get; set; }
    public Endereco? Endereco { get; set; }

    public DadosPessoaFisica? PessoaFisica { get; set; }
    public DadosPessoaJuridica? PessoaJuridica { get; set; }
    public Guid? OrigemId { get; set; }
    public Origem? Origem { get; set; }

    public Conjuge? Conjuge { get; set; }
    public DadosComplementares? DadosComplementares { get; set; }

    public ICollection<Contato> Contatos { get; set; } = new List<Contato>();
    public ICollection<ContratoParte> Contratos { get; set; } = new List<ContratoParte>();

    public Guid? ImobiliariaId { get; set; }
    public Imobiliaria? Imobiliaria { get; set; }
}
