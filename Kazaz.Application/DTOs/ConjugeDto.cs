using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ConjugeDto(
	Guid? Id,
	string Nome,
	string Cpf,
	string? Rg,
	string? OrgaoExpedidor,
	DateOnly? DataNascimento,
	string? Telefone,
	string? Email
);

public sealed record ConjugeUpdateDto(
    string? Nome,
    string? Documento
);
