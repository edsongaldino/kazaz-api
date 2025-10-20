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

public class VinculoImovelService(ApplicationDbContext ctx) : IVinculoImovelService
{
    public async Task VincularAsync(VincularPessoaImovelDto dto, CancellationToken ct)
    {
        // valida existência
        var pessoa = await ctx.Set<Pessoa>().AsNoTracking().AnyAsync(p => p.Id == dto.PessoaId, ct);
        if (!pessoa) throw new KeyNotFoundException("Pessoa não encontrada.");
        var imovel = await ctx.Set<Imovel>().AsNoTracking().AnyAsync(i => i.Id == dto.ImovelId, ct);
        if (!imovel) throw new KeyNotFoundException("Imóvel não encontrado.");
        var perfil = await ctx.Set<PerfilVinculoImovel>().AsNoTracking().AnyAsync(p => p.Id == dto.PerfilVinculoImovelId, ct);
        if (!perfil) throw new KeyNotFoundException("Perfil de vínculo não encontrado.");

        var jaExiste = await ctx.Set<VinculoPessoaImovel>().AsNoTracking()
            .AnyAsync(v => v.PessoaId == dto.PessoaId && v.ImovelId == dto.ImovelId && v.PerfilVinculoImovelId == dto.PerfilVinculoImovelId, ct);
        if (jaExiste) return;

        ctx.Add(new VinculoPessoaImovel
        {
            PessoaId = dto.PessoaId,
            ImovelId = dto.ImovelId,
            PerfilVinculoImovelId = dto.PerfilVinculoImovelId
        });

        await ctx.SaveChangesAsync(ct);
    }

    public async Task DesvincularAsync(Guid pessoaId, Guid imovelId, Guid? perfilVinculoImovelId, CancellationToken ct)
    {
        var q = ctx.Set<VinculoPessoaImovel>().Where(v => v.PessoaId == pessoaId && v.ImovelId == imovelId);
        if (perfilVinculoImovelId.HasValue)
            q = q.Where(v => v.PerfilVinculoImovelId == perfilVinculoImovelId);

        var itens = await q.ToListAsync(ct);
        if (itens.Count == 0) return;

        ctx.RemoveRange(itens);
        await ctx.SaveChangesAsync(ct);
    }
}