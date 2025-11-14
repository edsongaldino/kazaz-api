using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class OrigemService : IOrigemService
{
    private readonly ApplicationDbContext _db;

    public OrigemService(ApplicationDbContext db) => _db = db;

    public async Task<OrigemResponseDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var o = await _db.Origens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Origem não encontrada.");

        return Map(o);
    }

    public async Task<PagedResult<OrigemResponseDto>> SearchAsync(string? q, int page, int pageSize, CancellationToken ct)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);

        var query = _db.Origens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Nome, $"%{term}%") ||
                                     EF.Functions.ILike(x.Descricao!, $"%{term}%"));
        }

        var total = await query.LongCountAsync(ct);
        var items = await query
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<OrigemResponseDto>
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<Guid> CreateAsync(OrigemCreateDto dto, CancellationToken ct)
    {
        // validação simples
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        var exists = await _db.Origens.AnyAsync(x => x.Nome.ToLower() == dto.Nome.Trim().ToLower(), ct);
        if (exists) throw new InvalidOperationException("Já existe uma origem com este nome.");

        var entity = new Origem
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao!.Trim()
        };

        _db.Origens.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(Guid id, OrigemUpdateDto dto, CancellationToken ct)
    {
        var o = await _db.Origens.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Origem não encontrada.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        var nameExists = await _db.Origens
            .AnyAsync(x => x.Id != id && x.Nome.ToLower() == dto.Nome.Trim().ToLower(), ct);
        if (nameExists) throw new InvalidOperationException("Já existe outra origem com este nome.");

        o.Nome = dto.Nome.Trim();
        o.Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao!.Trim();

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var o = await _db.Origens
            .Include(x => x.Clientes)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Origem não encontrada.");

        if (o.Clientes.Any())
            throw new InvalidOperationException("Não é possível excluir: existem clientes vinculados a esta origem.");

        _db.Origens.Remove(o);
        await _db.SaveChangesAsync(ct);
    }

    private static OrigemResponseDto Map(Origem o) =>
        new(o.Id, o.Nome, o.Descricao);
}
