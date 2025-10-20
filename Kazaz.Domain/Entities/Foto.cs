using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class Foto
{
    public Guid Id { get; set; }
    public Guid ImovelId { get; set; }
    public string Caminho { get; set; } = default!;
    public int Ordem { get; set; }


    public Imovel Imovel { get; set; } = default!;
}
