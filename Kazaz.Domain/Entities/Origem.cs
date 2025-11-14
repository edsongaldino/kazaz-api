using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Origem
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;
    public string? Descricao { get; set; }


    // Navegação inversa (opcional)
    public ICollection<Pessoa> Clientes { get; set; } = new List<Pessoa>();
}
