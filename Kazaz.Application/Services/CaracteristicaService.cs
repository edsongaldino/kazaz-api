using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

public sealed class CaracteristicaService : ICaracteristicaService
{
    private readonly ApplicationDbContext _db;
    public CaracteristicaService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<CaracteristicaListDto>> ListarAsync(bool? ativo, string? grupo, CancellationToken ct)
    {
        var q = _db.Set<Caracteristica>()
            .AsNoTracking();

        if (ativo is not null)
            q = q.Where(x => x.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(grupo))
        {
            var g = grupo.Trim();
            q = q.Where(x => x.Grupo != null && x.Grupo == g);
        }

        return await q
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .Select(x => new CaracteristicaListDto(
                x.Id,
                x.Nome,
                x.TipoValor,
                x.Unidade,
                x.Grupo,
                x.Ordem,
                x.Ativo
            ))
            .ToListAsync(ct);
    }
}
