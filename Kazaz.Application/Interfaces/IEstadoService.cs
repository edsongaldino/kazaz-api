using Kazaz.Application.DTOs;

public interface IEstadoService
{
    Task<IReadOnlyList<EstadoDto>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<EstadoDto>> SearchAsync(string? q, CancellationToken ct);
    Task<EstadoDto?> GetByUfAsync(string uf, CancellationToken ct);
    Task<EstadoDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<CidadeSlimDto>> ListCidadesAsync(Guid estadoId, CancellationToken ct);
}
