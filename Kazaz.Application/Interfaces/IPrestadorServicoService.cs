using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IPrestadorServicoService
{
    Task<PrestadorServicoResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<PrestadorServicoResponseDto> Items, int Total)> ListarAsync(PrestadorServicoSearchFilterDto filtro, CancellationToken ct);
    Task<Guid> CriarAsync(PrestadorServicoCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, PrestadorServicoUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}
