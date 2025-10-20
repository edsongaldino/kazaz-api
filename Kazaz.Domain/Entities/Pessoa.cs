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


    public List<VinculoPessoaImovel> Vinculos { get; set; } = new();
}
