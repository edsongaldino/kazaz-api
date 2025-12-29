using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class Caracteristica
{
	public Guid Id { get; set; }
	public string Nome { get; set; } = null!;
	public string TipoValor { get; set; } = null!; // "bool","int","decimal","texto","data","moeda"
	public string? Unidade { get; set; }
	public string? Grupo { get; set; }
	public int Ordem { get; set; } = 0;
	public bool Ativo { get; set; } = true;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<ImovelCaracteristica> ImoveisCaracteristicas { get; set; } = new List<ImovelCaracteristica>();
}

