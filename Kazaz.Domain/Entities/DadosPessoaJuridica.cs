using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class DadosPessoaJuridica : Pessoa
{
    public string Cnpj { get; set; } = default!;
    public string RazaoSocial { get; set; } = default!;
    public DateOnly? DataAbertura { get; set; }

    public List<Socio> Socios { get; set; } = new();
    public Pessoa Pessoa { get; set; } = null!;
}