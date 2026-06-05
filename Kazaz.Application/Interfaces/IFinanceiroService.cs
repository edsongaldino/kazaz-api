using System;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IFinanceiroService
{
    Task<FinanceiroLancamentoResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<FinanceiroLancamentoResponseDto> Items, int Total)> ListarAsync(FinanceiroLancamentoSearchFilterDto filtro, CancellationToken ct);
    Task<Guid> CriarAsync(FinanceiroLancamentoCreateDto dto, CancellationToken ct);
    Task AtualizarAsync(Guid id, FinanceiroLancamentoUpdateDto dto, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
    Task<FinanceiroResumoDto> ObterResumoFinanceiroAsync(CancellationToken ct);
}
