using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Domain.Entities;

public class ImovelCaracteristica
{
	public Guid Id { get; set; }

	public Guid ImovelId { get; set; }
	public Imovel Imovel { get; set; } = null!;

	public Guid CaracteristicaId { get; set; }
	public Caracteristica Caracteristica { get; set; } = null!;

	public bool? ValorBool { get; set; }
	public int? ValorInt { get; set; }
	public decimal? ValorDecimal { get; set; }
	public string? ValorTexto { get; set; }
	public DateOnly? ValorData { get; set; }

	public string? Observacao { get; set; }
}

