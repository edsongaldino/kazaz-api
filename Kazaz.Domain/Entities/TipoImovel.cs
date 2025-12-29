using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class TipoImovel
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = null!;
	public bool Ativo { get; set; } = true;
	public int Ordem { get; set; } = 0;
	public string? Categoria { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<Imovel> Imoveis { get; set; } = new List<Imovel>();
}
