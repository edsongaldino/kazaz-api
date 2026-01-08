using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Pessoa
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
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
}
