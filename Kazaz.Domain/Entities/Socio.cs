using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class Socio
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Guid PessoaFisicaId { get; set; }


    public DadosPessoaJuridica Empresa { get; set; } = default!;
    public DadosPessoaFisica PessoaFisica { get; set; } = default!;
    public decimal? Percentual { get; set; }
}
