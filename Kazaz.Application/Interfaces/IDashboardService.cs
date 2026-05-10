using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumoDto> ObterResumoAsync(CancellationToken ct);
}