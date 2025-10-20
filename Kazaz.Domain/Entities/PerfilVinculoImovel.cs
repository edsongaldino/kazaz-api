using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class PerfilVinculoImovel
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }

    public ICollection<VinculoPessoaImovel>? Vinculos { get; set; }
}
