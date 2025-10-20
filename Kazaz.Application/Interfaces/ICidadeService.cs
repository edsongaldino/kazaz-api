using Kazaz.Application.DTOs;

public interface ICidadeService
{
    Task<IReadOnlyList<CidadeDto>> ListAsync(Guid? estadoId, CancellationToken ct);
    Task<IReadOnlyList<CidadeDto>> SearchAsync(string? q, Guid? estadoId, CancellationToken ct);
    Task<CidadeDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<CidadeDto?> GetByIbgeAsync(string ibge, CancellationToken ct);
}
