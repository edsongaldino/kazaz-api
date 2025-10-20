using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class PessoaService(ApplicationDbContext ctx) : IPessoaService
{
    public async Task<(IReadOnlyList<PessoaListDto> Items, int Total)> ListarAsync(
        int page, int pageSize, string? termo, CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var t = (termo ?? string.Empty).Trim();
        var digits = new string(t.Where(char.IsDigit).ToArray());

        // PF
        var pfQ = ctx.Set<DadosPessoaFisica>().AsNoTracking();
        if (!string.IsNullOrEmpty(t))
        {
            pfQ = pfQ.Where(p =>
                EF.Functions.ILike(p.Nome, $"%{t}%") ||
                (!string.IsNullOrEmpty(digits) && EF.Functions.ILike(p.Cpf, $"%{digits}%"))
            );
        }
        var pfProj = pfQ.Select(p => new
        {
            p.Id,
            Nome = p.Nome,
            Tipo = "FISICA",
            Documento = p.Cpf,
            Nascimento = (DateOnly?)p.DataNascimento,
            RazaoSocial = (string?)null,
            EnderecoId = p.EnderecoId
        });

        // PJ
        var pjQ = ctx.Set<DadosPessoaJuridica>().AsNoTracking();
        if (!string.IsNullOrEmpty(t))
        {
            pjQ = pjQ.Where(p =>
                EF.Functions.ILike(p.Nome, $"%{t}%") ||
                EF.Functions.ILike(p.RazaoSocial, $"%{t}%") ||
                (!string.IsNullOrEmpty(digits) && EF.Functions.ILike(p.Cnpj, $"%{digits}%"))
            );
        }
        var pjProj = pjQ.Select(p => new
        {
            p.Id,
            Nome = p.Nome,
            Tipo = "JURIDICA",
            Documento = p.Cnpj,
            Nascimento = (DateOnly?)null,
            RazaoSocial = (string?)p.RazaoSocial,
            EnderecoId = p.EnderecoId
        });

        // Union/concat no servidor
        var unionQ = pfProj.Concat(pjProj);

        var total = await unionQ.CountAsync(ct);

        var items = await unionQ
            .OrderBy(x => x.Nome)
            .ThenBy(x => x.Tipo) // para estabilizar
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PessoaListDto(
                x.Id, x.Tipo, x.Nome, x.Documento, x.Nascimento, x.RazaoSocial, x.EnderecoId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        // tenta PF
        var pf = await ctx.Set<DadosPessoaFisica>().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaListDto(
                p.Id, "FISICA", p.Nome, p.Cpf, p.DataNascimento, null, p.EnderecoId))
            .FirstOrDefaultAsync(ct);

        if (pf is not null) return pf;

        // tenta PJ
        var pj = await ctx.Set<DadosPessoaJuridica>().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaListDto(
                p.Id, "JURIDICA", p.Nome, p.Cnpj, null, p.RazaoSocial, p.EnderecoId))
            .FirstOrDefaultAsync(ct);

        return pj;
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        // remove a pessoa inteira (independente do tipo)
        var pessoaBase = await ctx.Set<Pessoa>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (pessoaBase is null) return;

        ctx.Remove(pessoaBase);
        await ctx.SaveChangesAsync(ct);
    }
}
