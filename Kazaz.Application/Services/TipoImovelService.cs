using Kazaz.Application.DTOs;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

public sealed class TipoImovelService : ITipoImovelService
{
    private readonly ApplicationDbContext _db;
    public TipoImovelService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<TipoImovelListDto>> ListarAsync(bool? ativo, string? categoria, CancellationToken ct)
    {
        var q = _db.Set<TipoImovel>()
            .AsNoTracking();

        if (ativo is not null)
            q = q.Where(x => x.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            var cat = categoria.Trim().ToUpperInvariant();
            q = q.Where(x => x.Categoria != null && x.Categoria.ToUpper() == cat);
        }

        return await q
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .Select(x => new TipoImovelListDto(
                x.Id,
                x.Nome,
                x.Ativo,
                x.Ordem,
                x.Categoria
            ))
            .ToListAsync(ct);
    }
}

