using Kazaz.Application.DTOs;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

public sealed class CidadeService : ICidadeService
{
    private readonly ApplicationDbContext _db;
    public CidadeService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CidadeDto>> ListAsync(Guid? estadoId, CancellationToken ct)
    {
        var query = _db.Cidades.AsNoTracking().Include(c => c.Estado).AsQueryable();
        if (estadoId.HasValue) query = query.Where(c => c.EstadoId == estadoId);
        return await query
            .OrderBy(c => c.Nome)
            .Select(c => new CidadeDto(c.Id, c.Nome, c.Ibge, c.EstadoId, c.Estado.Uf))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CidadeDto>> SearchAsync(string? q, Guid? estadoId, CancellationToken ct)
    {
        var query = _db.Cidades.AsNoTracking().Include(c => c.Estado).AsQueryable();
        if (estadoId.HasValue) query = query.Where(c => c.EstadoId == estadoId);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim();
            query = query.Where(c =>
                EF.Functions.ILike(c.Nome, $"%{s}%") ||
                EF.Functions.ILike(c.Ibge, $"%{s}%"));
        }
        return await query
            .OrderBy(c => c.Nome)
            .Select(c => new CidadeDto(c.Id, c.Nome, c.Ibge, c.EstadoId, c.Estado.Uf))
            .ToListAsync(ct);
    }

    public async Task<CidadeDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _db.Cidades.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CidadeDto(c.Id, c.Nome, c.Ibge, c.EstadoId, c.Estado.Uf))
            .FirstOrDefaultAsync(ct);

    public async Task<CidadeDto?> GetByIbgeAsync(string ibge, CancellationToken ct)
        => await _db.Cidades.AsNoTracking()
            .Where(c => c.Ibge == ibge)
            .Select(c => new CidadeDto(c.Id, c.Nome, c.Ibge, c.EstadoId, c.Estado.Uf))
            .FirstOrDefaultAsync(ct);
}
