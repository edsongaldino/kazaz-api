using Kazaz.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.Application.Interfaces
{
    public interface IPessoaService
    {
        Task<(IReadOnlyList<PessoaListDto> Items, int Total)> ListarAsync(
            int page,
            int pageSize,
            string? termo,
            CancellationToken ct = default);

        Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct = default);

        Task RemoverAsync(Guid id, CancellationToken ct = default);
    }
}
