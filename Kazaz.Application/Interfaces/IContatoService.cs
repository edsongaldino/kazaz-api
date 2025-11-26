using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IContatoService
{
	Task CriarVariosAsync(Guid pessoaId, IEnumerable<ContatoCreateDto> contatos, CancellationToken ct);
}
