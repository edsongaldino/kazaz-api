using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IColaboradorService
{
    Task<ColaboradorResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<ColaboradorResponseDto> Items, int Total)> ListarAsync(ColaboradorSearchFilterDto filtro, CancellationToken ct);
    Task<Guid> CriarAsync(ColaboradorCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, ColaboradorUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}
