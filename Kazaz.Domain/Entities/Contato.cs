using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Contato
{
    public Guid Id { get; set; }

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;

    public string Tipo { get; set; } = null!;   // ou um enum se preferir
    public string Valor { get; set; } = null!;
    public bool Principal { get; set; }

    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
