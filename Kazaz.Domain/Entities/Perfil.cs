using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Perfil
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = default!;

    // opcional: se quiser navegar do Perfil -> Usuarios
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
