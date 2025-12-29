using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class DadosPessoaJuridica : Pessoa
{
    public Guid Id { get; set; }          // PK e FK (shared)
    public Pessoa Pessoa { get; set; } = null!;

    public string Cnpj { get; set; } = default!;
    public string RazaoSocial { get; set; } = default!;
    public string NomeFantasia { get; set; } = default!;
    public string InscricaoEstadual { get; set; } = default!;
    public DateOnly? DataAbertura { get; set; }

    public List<Socio> Socios { get; set; } = new();
}