using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public sealed class Imovel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = default!;
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = default!;


    public List<VinculoPessoaImovel> Vinculos { get; set; } = new();
    public List<Foto> Fotos { get; set; } = new();
}
