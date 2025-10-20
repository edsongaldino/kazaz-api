using Microsoft.EntityFrameworkCore;
using Kazaz.Application.DTOs;
using Kazaz.Infrastructure.Data;

public sealed class EstadoService : IEstadoService
{
    private readonly ApplicationDbContext _db;
    public EstadoService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<EstadoDto>> ListAsync(CancellationToken ct)
        => await _db.Estados.AsNoTracking()
            .OrderBy(x => x.Nome)
            .Select(x => new EstadoDto(x.Id, x.Nome, x.Uf))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<EstadoDto>> SearchAsync(string? q, CancellationToken ct)
    {
        var query = _db.Estados.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Nome, $"%{s}%") ||
                EF.Functions.ILike(x.Uf, $"%{s}%"));
        }

        return await query
            .OrderBy(x => x.Nome)
            .Select(x => new EstadoDto(x.Id, x.Nome, x.Uf))
            .ToListAsync(ct);
    }

    public async Task<EstadoDto?> GetByUfAsync(string uf, CancellationToken ct)
        => await _db.Estados.AsNoTracking()
            .Where(x => x.Uf == uf.ToUpper())
            .Select(x => new EstadoDto(x.Id, x.Nome, x.Uf))
            .FirstOrDefaultAsync(ct);

    public async Task<EstadoDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Estados.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new EstadoDto(x.Id, x.Nome, x.Uf))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CidadeSlimDto>> ListCidadesAsync(Guid estadoId, CancellationToken ct)
        => await _db.Cidades.AsNoTracking()
            .Where(c => c.EstadoId == estadoId)
            .OrderBy(c => c.Nome)
            .Select(c => new CidadeSlimDto(c.Id, c.Nome))
            .ToListAsync(ct);

}
