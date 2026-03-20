using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class PessoaJuridicaService(ApplicationDbContext ctx) : IPessoaJuridicaService
{
    public async Task<Guid> CriarAsync(Guid pessoaId, DadosPessoaJuridicaDto dto, CancellationToken ct)
    {
        var cnpj = LimparCnpj(dto.Cnpj);

        var existe = await ctx.Set<DadosPessoaJuridica>()
            .AnyAsync(x => x.Cnpj == cnpj, ct);
        if (existe) throw new InvalidOperationException("CNPJ já cadastrado.");

        var ent = new DadosPessoaJuridica
        {
            PessoaId = pessoaId,
            NomeFantasia = dto.NomeFantasia.Trim(),
            RazaoSocial = dto.RazaoSocial.Trim(),
            DataAbertura = dto.DataAbertura,
            Cnpj = cnpj,
            InscricaoEstadual = dto.InscricaoEstadual
        };
        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);
        return ent.PessoaId;
    }

    public async Task AtualizarAsync(Guid id, PessoaJuridicaUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<DadosPessoaJuridica>()
            .FirstOrDefaultAsync(x => x.PessoaId == id, ct)
            ?? throw new KeyNotFoundException("Pessoa Jurídica não encontrada.");

        var cnpj = LimparCnpj(dto.Cnpj);
        var existeOutro = await ctx.Set<DadosPessoaJuridica>()
            .AnyAsync(x => x.PessoaId != id && x.Cnpj == cnpj, ct);
        if (existeOutro) throw new InvalidOperationException("CNPJ já cadastrado.");

        ent.RazaoSocial = dto.RazaoSocial.Trim();
        ent.Cnpj = cnpj;
        ent.DataAbertura = dto.DataAbertura;
        ent.InscricaoEstadual = dto.InscricaoEstadual;
        ent.NomeFantasia = dto.NomeFantasia;


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
                EF.Functions.ILike(p.NomeFantasia, $"%{t}%") ||
                EF.Functions.ILike(p.RazaoSocial, $"%{t}%") ||
                (!string.IsNullOrEmpty(digits) && EF.Functions.ILike(p.Cnpj, $"%{digits}%"))
            );
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(p => p.NomeFantasia)
            .ThenBy(p => p.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PessoaListDto(
                p.PessoaId, "JURIDICA", p.NomeFantasia, p.Cnpj, null, p.RazaoSocial, p.Pessoa.EnderecoId, p.Pessoa.OrigemId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        return await ctx.Set<DadosPessoaJuridica>().AsNoTracking()
            .Where(p => p.PessoaId == id)
            .Select(p => new PessoaListDto(
                p.PessoaId, "JURIDICA", p.NomeFantasia, p.Cnpj, null, p.RazaoSocial, p.Pessoa.EnderecoId, p.Pessoa.OrigemId))
            .FirstOrDefaultAsync(ct);
    }

    static string LimparCnpj(string cnpj) =>
        new string((cnpj ?? string.Empty).Where(char.IsDigit).ToArray()).PadLeft(14, '0');
}
