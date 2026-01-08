using Kazaz.Application.DTOs;
using Microsoft.EntityFrameworkCore;

public interface ITipoImovelService
{
    Task<IReadOnlyList<TipoImovelListDto>> ListarAsync(bool? ativo, string? categoria, CancellationToken ct);
}