using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class PessoaFisicaService(ApplicationDbContext ctx) : IPessoaFisicaService
{
    public async Task<Guid> CriarAsync(DadosPessoaFisicaDto dto, CancellationToken ct)
    {
        var cpf = LimparCpf(dto.Cpf);

        // (opcional) garantir unicidade via app além do índice único
        var existe = await ctx.Set<DadosPessoaFisica>()
            .AnyAsync(x => x.Cpf == cpf, ct);
        if (existe) throw new InvalidOperationException("CPF já cadastrado.");

        var ent = new DadosPessoaFisica
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Cpf = cpf,
            DataNascimento = dto.DataNascimento
        };

        ctx.Add(ent);
        await ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task AtualizarAsync(Guid id, PessoaFisicaUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<DadosPessoaFisica>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Pessoa Física não encontrada.");

        var cpf = LimparCpf(dto.Cpf);

        // (opcional) confirmar unicidade
        var existeOutro = await ctx.Set<DadosPessoaFisica>()
            .AnyAsync(x => x.Id != id && x.Cpf == cpf, ct);
        if (existeOutro) throw new InvalidOperationException("CPF já cadastrado.");

        ent.Nome = dto.Nome.Trim();
        ent.Cpf = cpf;
        ent.DataNascimento = dto.DataNascimento;
        ent.OrgaoExpedidor = dto.OrgaoExpedidor;

        await ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        // remover a pessoa inteira (base + filha)
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

        var q = ctx.Set<DadosPessoaFisica>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var t = termo.Trim();
            var digits = new string(t.Where(char.IsDigit).ToArray());

            q = q.Where(p =>
                EF.Functions.ILike(p.Nome, $"%{t}%") ||
                (!string.IsNullOrEmpty(digits) && EF.Functions.ILike(p.Cpf, $"%{digits}%"))
            );
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PessoaListDto(
                p.Id, "FISICA", p.Nome, p.Cpf, p.DataNascimento, null, p.EnderecoId, p.OrigemId))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<PessoaListDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        return await ctx.Set<DadosPessoaFisica>().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PessoaListDto(
                p.Id, "FISICA", p.Nome, p.Cpf, p.DataNascimento, null, p.EnderecoId, p.OrigemId))
            .FirstOrDefaultAsync(ct);
    }

    static string LimparCpf(string cpf) =>
        new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray()).PadLeft(11, '0');
}
