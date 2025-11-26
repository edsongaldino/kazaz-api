using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;

public record ConjugeCreateDto(
	string Nome,
	string Cpf,
	string? Rg,
	string? OrgaoExpedidor,
	DateOnly? DataNascimento,
	string? Telefone,
	string? Email
);
