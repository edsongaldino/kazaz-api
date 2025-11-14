using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class PessoaJuridicaService(ApplicationDbContext ctx) : IPessoaJuridicaService
{
    public async Task<Guid> CriarAsync(PessoaJuridicaCreateDto dto, CancellationToken ct)
    {
        var cnpj = LimparCnpj(dto.Cnpj);

        var existe = await ctx.Set<DadosPessoaJuridica>()
            .AnyAsync(x => x.Cnpj == cnpj, ct);
        if (existe) throw new InvalidOperationException("CNPJ já cadastrado.");

        var ent = new DadosPessoaJuridica
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            RazaoSocial = dto.RazaoSocial.Trim(),
            Cnpj = cnpj,
            EnderecoId = dto.EnderecoId
        };
        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task AtualizarAsync(Guid id, PessoaJuridicaUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<DadosPessoaJuridica>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Pessoa Jurídica não encontrada.");

        var cnpj = LimparCnpj(dto.Cnpj);
        var existeOutro = await ctx.Set<DadosPessoaJuridica>()
            .AnyAsync(x => x.Id != id && x.Cnpj == cnpj, ct);
        if (existeOutro) throw new InvalidOperationException("CNPJ já cadastrado.");

        ent.Nome = dto.Nome.Trim();
        ent.RazaoSocial = dto.RazaoSocial.Trim();
        ent.Cnpj = cnpj;
        ent.EnderecoId = dto.EnderecoId;

        await ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var pessoaBase = await ctx.Set<Pessoa>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (pessoaBase is null) return;

        ctx.Remove(pessoaBase);
        await ctx.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<PessoaListDto> Items, int Total)> ListarAsync(
        int page, int pageSize, string? termo, CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var q = ctx.Set<DadosPessoaJuridica>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var t = termo.Trim();
            var digits = new string(t.Where(char.IsDigit).ToArray());

            q = q.Where(p =>
                EF.Functions.ILike(p.Nome, $"%{t}%") ||
                EF.Functions.ILike(p.RazaoSocial, $"%{t}%") ||
                (!string.IsNullOrEmpty(digits) && EF.Functions.ILike(p.Cnpj, $"%{digits}%"))
            );
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(p => p.Nome)
            .ThenBy(p => p.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PessoaListDto(
                p.Id, "JURIDICA", p.Nome, p.Cnpj, null, p.RazaoSocial, p.EnderecoId, p.OrigemId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        return await ctx.Set<DadosPessoaJuridica>().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaListDto(
                p.Id, "JURIDICA", p.Nome, p.Cnpj, null, p.RazaoSocial, p.EnderecoId, p.OrigemId))
            .FirstOrDefaultAsync(ct);
    }

    static string LimparCnpj(string cnpj) =>
        new string((cnpj ?? string.Empty).Where(char.IsDigit).ToArray()).PadLeft(14, '0');
}
