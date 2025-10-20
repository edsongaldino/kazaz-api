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

public class FotoService(ApplicationDbContext ctx) : IFotoService
{
    public async Task<IReadOnlyList<FotoListDto>> ListarPorImovelAsync(Guid imovelId, CancellationToken ct)
        => await ctx.Set<Foto>().AsNoTracking()
            .Where(f => f.ImovelId == imovelId)
            .OrderBy(f => f.Ordem)
            .Select(f => new FotoListDto(f.Id, f.ImovelId, f.Caminho, f.Ordem))
            .ToListAsync(ct);

    public async Task<FotoListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        var f = await ctx.Set<Foto>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return f is null ? null : new FotoListDto(f.Id, f.ImovelId, f.Caminho, f.Ordem);
    }

    public async Task<Guid> CriarAsync(FotoCreateDto dto, CancellationToken ct)
    {
        var existeImovel = await ctx.Set<Imovel>().AsNoTracking().AnyAsync(i => i.Id == dto.ImovelId, ct);
        if (!existeImovel) throw new KeyNotFoundException("Imóvel não encontrado.");

        var ent = new Foto
        {
            Id = Guid.NewGuid(),
            ImovelId = dto.ImovelId,
            Caminho = dto.Caminho.Trim(),
            Ordem = dto.Ordem
        };
        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task AtualizarAsync(Guid id, FotoUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<Foto>().FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Foto não encontrada.");
        ent.Caminho = dto.Caminho.Trim();
        ent.Ordem = dto.Ordem;
        await ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var ent = await ctx.Set<Foto>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (ent is null) return;
        ctx.Remove(ent);
        await ctx.SaveChangesAsync(ct);
    }

    public async Task ReordenarAsync(Guid imovelId, IEnumerable<(Guid fotoId, int ordem)> novaOrdem, CancellationToken ct)
    {
        var ids = novaOrdem.Select(x => x.fotoId).ToHashSet();
        var fotos = await ctx.Set<Foto>().Where(f => f.ImovelId == imovelId && ids.Contains(f.Id)).ToListAsync(ct);
        var map = novaOrdem.ToDictionary(x => x.fotoId, x => x.ordem);
        foreach (var f in fotos) f.Ordem = map[f.Id];
        await ctx.SaveChangesAsync(ct);
    }
}
