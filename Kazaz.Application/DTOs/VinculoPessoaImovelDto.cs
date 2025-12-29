using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.DTOs;
public sealed record VinculoPessoaImovelDto(
	Guid Id,
	Guid PessoaId,
	string PessoaNome,
	Guid PerfilVinculoImovelId,
	string PerfilNome
);

public sealed record VinculoPessoaImovelUpsertDto(
	Guid PessoaId,
	Guid PerfilVinculoImovelId
);
