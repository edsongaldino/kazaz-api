using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kazaz.Application.Services;

public class PessoaService(ApplicationDbContext ctx) : IPessoaService
{
    public async Task<(IReadOnlyList<PessoaListDto> Items, int Total)> ListarAsync(
    PessoaFiltroDto filtro,
    CancellationToken ct)
    {
        var page = Math.Max(1, filtro.Page);
        var pageSize = Math.Max(1, filtro.PageSize);

        var nome = (filtro.Nome ?? string.Empty).Trim();
        var documento = (filtro.Documento ?? string.Empty).Trim();
        var digits = new string(documento.Where(char.IsDigit).ToArray());

        // =====================
        // PF
        // =====================
        var pfQ = ctx.Set<DadosPessoaFisica>().AsNoTracking();

        if (!string.IsNullOrEmpty(nome))
        {
            pfQ = pfQ.Where(p =>
                EF.Functions.ILike(p.Nome, $"%{nome}%"));
        }

        if (!string.IsNullOrEmpty(digits))
        {
            pfQ = pfQ.Where(p =>
                EF.Functions.ILike(p.Cpf, $"%{digits}%"));
        }

        var pfProj = pfQ.Select(p => new
        {
            p.PessoaId,
            Nome = p.Nome,
            Tipo = "FISICA", // Fisica
            Documento = p.Cpf,
            Nascimento = (DateOnly?)p.DataNascimento,
            RazaoSocial = (string?)null,
            EnderecoId = p.Pessoa.EnderecoId,
            OrigemId = p.Pessoa.OrigemId,

            QuantidadeContratos = p.Pessoa.Contratos
                .Select(c => c.ContratoId)
                .Distinct()
                .Count(),

            EhLocador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Locador),
            EhLocatario = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Locatario),
            EhFiador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Fiador),
            EhVendedor = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Vendedor),
            EhComprador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Comprador)
        });

        // =====================
        // PJ
        // =====================
        var pjQ = ctx.Set<DadosPessoaJuridica>().AsNoTracking();

        if (!string.IsNullOrEmpty(nome))
        {
            pjQ = pjQ.Where(p =>
                EF.Functions.ILike(p.NomeFantasia, $"%{nome}%") ||
                EF.Functions.ILike(p.RazaoSocial, $"%{nome}%"));
        }

        if (!string.IsNullOrEmpty(digits))
        {
            pjQ = pjQ.Where(p =>
                EF.Functions.ILike(p.Cnpj, $"%{digits}%"));
        }

        var pjProj = pjQ.Select(p => new
        {
            p.PessoaId,
            Nome = p.NomeFantasia,
            Tipo = "JURIDICA", // Juridica
            Documento = p.Cnpj,
            Nascimento = (DateOnly?)null,
            RazaoSocial = p.RazaoSocial,
            EnderecoId = p.Pessoa.EnderecoId,
            OrigemId = p.Pessoa.OrigemId,

            QuantidadeContratos = p.Pessoa.Contratos
                .Select(c => c.ContratoId)
                .Distinct()
                .Count(),

            EhLocador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Locador),
            EhLocatario = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Locatario),
            EhFiador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Fiador),
            EhVendedor = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Vendedor),
            EhComprador = p.Pessoa.Contratos.Any(c => c.Papel == PapelContrato.Comprador)
        });

        var unionQ = pfProj.Concat(pjProj);

        // =====================
        // FILTRO TIPO
        // =====================
        if (!string.IsNullOrWhiteSpace(filtro.Tipo))
        {
            var tipoCadastro = filtro.Tipo.Trim().ToUpper();

            unionQ = unionQ.Where(x => x.Tipo == tipoCadastro);
        }

        // =====================
        // FILTRO PAPEL
        // =====================
        if (filtro.Papel.HasValue)
        {
            unionQ = unionQ.Where(x =>
                (filtro.Papel == (int)PapelContrato.Locador && x.EhLocador) ||
                (filtro.Papel == (int)PapelContrato.Locatario && x.EhLocatario) ||
                (filtro.Papel == (int)PapelContrato.Fiador && x.EhFiador) ||
                (filtro.Papel == (int)PapelContrato.Vendedor && x.EhVendedor) ||
                (filtro.Papel == (int)PapelContrato.Comprador && x.EhComprador)
            );
        }

        var total = await unionQ.CountAsync(ct);

        var items = await unionQ
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PessoaListDto(
                x.PessoaId,
                x.Tipo,
                x.Nome,
                x.Documento,
                x.Nascimento,
                x.RazaoSocial,
                x.EnderecoId,
                x.OrigemId,
                x.QuantidadeContratos,
                x.EhLocador,
                x.EhLocatario,
                x.EhFiador,
                x.EhVendedor,
                x.EhComprador
            ))
            .ToListAsync(ct);

        return (items, total);
    }


    public async Task<PessoaDetailsDto?> ObterAsync(Guid id, CancellationToken ct)
    {
        var p = await ctx.Set<Pessoa>()
            .AsNoTracking()
            .Include(x => x.Endereco)
            .Include(x => x.PessoaFisica)
            .Include(x => x.PessoaJuridica)
            .Include(x => x.DadosComplementares)
            .Include(x => x.Conjuge)
            .Include(x => x.Contatos)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (p is null) return null;

        var isPf = p.PessoaFisica is not null;
        var isPj = p.PessoaJuridica is not null;

        // Se acontecer de não ter nenhum (base órfã), decide sua regra:
        // aqui eu devolvo como "PF" por padrão? Melhor devolver null ou lançar exceção.
        // Vou devolver null para o painel não quebrar (você pode trocar por throw).
        if (!isPf && !isPj)
            return null;

        var nome = isPf
            ? p.PessoaFisica!.Nome
            : (p.PessoaJuridica!.NomeFantasia ?? p.PessoaJuridica!.RazaoSocial);

        var documento = isPf
            ? p.PessoaFisica!.Cpf
            : p.PessoaJuridica!.Cnpj;

        return new PessoaDetailsDto(
            Id: p.Id,
            TipoPessoa: isPf ? "PF" : "PJ",
            Nome: nome,
            Documento: documento,
            OrigemId: p.OrigemId,

            Endereco: p.Endereco is null ? null : new EnderecoResponseDto
            {
                Id = p.Endereco.Id,
                Cep = p.Endereco.Cep,
                Logradouro = p.Endereco.Logradouro,
                Numero = p.Endereco.Numero,
                Complemento = p.Endereco.Complemento,
                Bairro = p.Endereco.Bairro,
                CidadeId = p.Endereco.CidadeId
            },

            DadosPessoaFisica: p.PessoaFisica is null ? null : new DadosPessoaFisicaDto(
                Nome: p.PessoaFisica.Nome,
                Cpf: p.PessoaFisica.Cpf,
                DataNascimento: p.PessoaFisica.DataNascimento,
                Rg: p.PessoaFisica.Rg,
                OrgaoExpedidor: p.PessoaFisica.OrgaoExpedidor,
                Nacionalidade: p.PessoaFisica.Nacionalidade,
                EstadoCivil: p.PessoaFisica.EstadoCivil
            ),

            DadosPessoaJuridica: p.PessoaJuridica is null ? null : new DadosPessoaJuridicaDto(
                RazaoSocial: p.PessoaJuridica.RazaoSocial,
                NomeFantasia: p.PessoaJuridica.NomeFantasia,
                Cnpj: p.PessoaJuridica.Cnpj,
                DataAbertura: p.PessoaJuridica.DataAbertura,
                InscricaoEstadual: p.PessoaJuridica.InscricaoEstadual
            ),

            Contatos: (p.Contatos ?? Enumerable.Empty<Contato>())
                .OrderByDescending(c => c.Principal)
                .ThenBy(c => c.Tipo)
                .Select(c => new ContatoDto(
                    Tipo: c.Tipo,
                    Valor: c.Valor,
                    Principal: c.Principal
                ))
                .ToList(),

            DadosComplementares: p.DadosComplementares is null ? null : new DadosComplementaresDto(
                Profissao: p.DadosComplementares.Profissao,
                Escolaridade: p.DadosComplementares.Escolaridade,
                RendaMensal: p.DadosComplementares.RendaMensal,
                Observacoes: p.DadosComplementares.Observacoes
            ),

            Conjuge: p.Conjuge is null ? null : new ConjugeDto(
                Id: p.Conjuge.Id,
                Nome: p.Conjuge.Nome,
                Cpf: p.Conjuge.Cpf,
                Rg: p.Conjuge.Rg,
                OrgaoExpedidor: p.Conjuge.OrgaoExpedidor,
                DataNascimento: p.Conjuge.DataNascimento,
                Telefone: p.Conjuge.Telefone,
                Email: p.Conjuge.Email
            )
        );
    }


    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        // remove a pessoa inteira (independente do tipo)
        var pessoaBase = await ctx.Set<Pessoa>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (pessoaBase is null) return;

        ctx.Remove(pessoaBase);
        await ctx.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Guid id, PessoaUpdateDto dto, Guid? enderecoId, CancellationToken ct)
    {
        var ent = await ctx.Set<Pessoa>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Pessoa não encontrada.");

        ent.EnderecoId = enderecoId;
        ent.OrigemId = dto.OrigemId;

        await ctx.SaveChangesAsync(ct);
    }

    public async Task<Guid> CriarBaseAsync(string nome, Guid? enderecoId, Guid? origemId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));

        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            EnderecoId = enderecoId,
            OrigemId = origemId
        };

        ctx.Pessoas.Add(pessoa);
        await ctx.SaveChangesAsync(ct);

        return pessoa.Id;
    }

    public async Task<PessoaDetailsDto?> ObterPorDocumentoAsync(string documentoDigits, CancellationToken ct)
    {
        Guid? pessoaId = null;

        if (documentoDigits.Length == 11)
        {
            pessoaId = await ctx.Set<DadosPessoaFisica>()
                .AsNoTracking()
                .Where(x => x.Cpf == documentoDigits)
                .Select(x => (Guid?)x.PessoaId)
                .FirstOrDefaultAsync(ct);
        }
        else if (documentoDigits.Length == 14)
        {
            pessoaId = await ctx.Set<DadosPessoaJuridica>()
                .AsNoTracking()
                .Where(x => x.Cnpj == documentoDigits)
                .Select(x => (Guid?)x.PessoaId)
                .FirstOrDefaultAsync(ct);
        }

        if (pessoaId is null) return null;

        return await ObterAsync(pessoaId.Value, ct);
    }

}
