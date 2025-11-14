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
            EnderecoId = p.EnderecoId,
            OrigemId = p.OrigemId
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
            EnderecoId = p.EnderecoId,
            OrigemId = p.OrigemId
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
                x.Id, x.Tipo, x.Nome, x.Documento, x.Nascimento, x.RazaoSocial, x.EnderecoId, x.OrigemId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<PessoaDetailsDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        var dto = await ctx.Set<Pessoa>()
        .AsNoTracking()
        .Where(p => p.Id == id)
        // garante que tenha ao menos um vínculo; se preferir, remova este Where
        .Where(p => p.PessoaFisica != null || p.PessoaJuridica != null)
        .Select(p => new PessoaDetailsDto(
            p.Id,

            p.PessoaFisica != null ? "PF" : "PJ",

            p.Nome,

            p.PessoaFisica != null ? p.PessoaFisica.Cpf : p.PessoaJuridica!.Cnpj,

            p.PessoaFisica != null ? p.PessoaFisica.DataNascimento : (DateOnly?)null,

            p.PessoaJuridica != null ? p.PessoaJuridica.RazaoSocial : null,

            p.Endereco == null ? null : new EnderecoResponseDto
            {
                Id = p.Endereco.Id,          // remova se seu DTO não tem Id
                Cep = p.Endereco.Cep,
                Logradouro = p.Endereco.Logradouro,
                Numero = p.Endereco.Numero,
                Complemento = p.Endereco.Complemento,
                Bairro = p.Endereco.Bairro,
                CidadeId = p.Endereco.CidadeId
            },
            p.OrigemId
        ))
        .FirstOrDefaultAsync(ct);

        return dto;
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
