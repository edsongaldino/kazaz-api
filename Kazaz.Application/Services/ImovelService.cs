using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Services;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class ImovelService : IImovelService
{
    private readonly ApplicationDbContext ctx;
    private readonly IEnderecoService _enderecoService;

    public ImovelService(ApplicationDbContext ctx, IEnderecoService enderecoService)
    {
        this.ctx = ctx;
        _enderecoService = enderecoService;
    }
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
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Codigo))
            throw new ArgumentException("Código é obrigatório.", nameof(dto));

        await using var tx = await ctx.Database.BeginTransactionAsync(ct);

        // 1) Resolver o EnderecoId
        Guid enderecoId;

        if (dto.Endereco is not null)
        {
            // Reaproveita seu serviço de Endereço
            var endResp = await _enderecoService.CriarAsync(dto.Endereco!);
            enderecoId = endResp.Id;
        }
        else
        {
            throw new ArgumentException("Informe EnderecoId ou o objeto Endereco.");
        }

        // 2) Criar o imóvel
        var ent = new Imovel
        {
            Id = Guid.NewGuid(),
            Codigo = dto.Codigo.Trim(),
            EnderecoId = enderecoId
        };

        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
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
