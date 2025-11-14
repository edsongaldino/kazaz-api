using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class DadosPessoaFisica : Pessoa
{
    public string Cpf { get; set; } = default!;
    public DateOnly? DataNascimento { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
}
