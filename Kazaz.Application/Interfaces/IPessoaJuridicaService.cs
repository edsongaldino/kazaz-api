using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces;

public interface IPessoaJuridicaService
{
    Task<Guid> CriarAsync(PessoaJuridicaCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, PessoaJuridicaUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}
