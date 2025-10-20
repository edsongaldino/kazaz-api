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

public class ImovelService(ApplicationDbContext ctx) : IImovelService
{
    public async Task<(IReadOnlyList<ImovelListDto> Items, int Total)> ListarAsync(int page, int pageSize, string? termo, CancellationToken ct)
    {
        page = page < 1 ? 1 : page; pageSize = pageSize < 1 ? 10 : pageSize;

        var q = ctx.Set<Imovel>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
            q = q.Where(i => EF.Functions.ILike(i.Codigo, $"%{termo}%"));

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(i => i.Codigo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new ImovelListDto(i.Id, i.Codigo, i.EnderecoId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<ImovelListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        var i = await ctx.Set<Imovel>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return i is null ? null : new ImovelListDto(i.Id, i.Codigo, i.EnderecoId);
    }

    public async Task<Guid> CriarAsync(ImovelCreateDto dto, CancellationToken ct)
    {
        var ent = new Imovel { Id = Guid.NewGuid(), Codigo = dto.Codigo.Trim(), EnderecoId = dto.EnderecoId };
        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task AtualizarAsync(Guid id, ImovelUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<Imovel>().FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Imóvel não encontrado.");
        ent.Codigo = dto.Codigo.Trim();
        ent.EnderecoId = dto.EnderecoId;
        await ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var ent = await ctx.Set<Imovel>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (ent is null) return;
        ctx.Remove(ent);
        await ctx.SaveChangesAsync(ct);
    }
}
