using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IConjugeService
{
	Task CriarOuAtualizarAsync(Guid pessoaId, ConjugeCreateDto dto, CancellationToken ct);
}
