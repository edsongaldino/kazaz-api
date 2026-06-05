using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IImobiliariaService
{
    Task<ImobiliariaResponseDto> ObterAsync(CancellationToken ct);
    Task<ImobiliariaResponseDto> SalvarAsync(ImobiliariaUpdateDto dto, CancellationToken ct);

    // Super Admin methods
    Task<(IReadOnlyList<ImobiliariaResponseDto> Items, int Total)> ListarTodasAsync(int page, int pageSize, string? termo, CancellationToken ct);
    Task<ImobiliariaResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<ImobiliariaResponseDto> CriarComAdminAsync(ImobiliariaCriarDto dto, CancellationToken ct);
    Task<ImobiliariaResponseDto> AtualizarPorIdAsync(Guid id, ImobiliariaUpdateDto dto, CancellationToken ct);
    Task<bool> ExcluirAsync(Guid id, CancellationToken ct);
}
