using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public sealed record ImovelCaracteristicaUpsertDto(
	Guid CaracteristicaId,
	bool? ValorBool,
	int? ValorInt,
	decimal? ValorDecimal,
	string? ValorTexto,
	DateOnly? ValorData,
	string? Observacao
);

public sealed record ImovelCaracteristicaDto(
	Guid Id,
	Guid CaracteristicaId,
	string CaracteristicaNome,
	string TipoValor,
	string? Unidade,
	string? Grupo,
	bool? ValorBool,
	int? ValorInt,
	decimal? ValorDecimal,
	string? ValorTexto,
	DateOnly? ValorData,
	string? Observacao
);

