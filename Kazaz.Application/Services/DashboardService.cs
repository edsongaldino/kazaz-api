using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext ctx;

    public DashboardService(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<DashboardResumoDto> ObterResumoAsync(CancellationToken ct)
    {
        var imoveis = ctx.Set<Imovel>().AsNoTracking();
        var pessoas = ctx.Set<Pessoa>().AsNoTracking();
        var contratos = ctx.Set<Contrato>().AsNoTracking();

        return new DashboardResumoDto
        {
            TotalImoveis = await imoveis.CountAsync(ct),
            TotalClientes = await pessoas.CountAsync(ct),
            TotalContratos = await contratos.CountAsync(ct),
            TotalConvites = await contratos
                .CountAsync(c => c.Status == StatusContrato.Rascunho, ct),
            TotalImobiliarias = await ctx.Set<Imobiliaria>().CountAsync(ct),
            TotalUsuarios = await ctx.Set<Usuario>().CountAsync(ct),

            ImoveisAtivos = await imoveis.CountAsync(x => (int)x.Status == 1, ct),
            ImoveisEmNegociacao = await imoveis.CountAsync(x => (int)x.Status == 3, ct),
            ImoveisVendidos = await imoveis.CountAsync(x => (int)x.Status == 4, ct),
            ImoveisAlugados = await imoveis.CountAsync(x => (int)x.Status == 5, ct),

            ImoveisPorTipo = await imoveis
                .GroupBy(x => x.TipoImovel)
                .Select(g => new DashboardGraficoItemDto
                {
                    Label = g.Key.Nome.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync(ct),

            ImoveisPorFinalidade = await imoveis
                .GroupBy(x => x.Finalidade)
                .Select(g => new DashboardGraficoItemDto
                {
                    Label = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync(ct),

            ConvitesPorStatus = await contratos
                .GroupBy(x => x.Status)
                .Select(g => new DashboardGraficoItemDto
                {
                    Label = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync(ct)
        };
    }
}