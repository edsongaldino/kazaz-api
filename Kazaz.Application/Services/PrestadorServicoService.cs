using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class PrestadorServicoService(ApplicationDbContext db) : IPrestadorServicoService
{
    public async Task<PrestadorServicoResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var prest = await db.PrestadoresServicos
            .AsNoTracking()
            .Include(x => x.Endereco)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Prestador de serviço não encontrado.");

        return Map(prest);
    }

    public async Task<(IReadOnlyList<PrestadorServicoResponseDto> Items, int Total)> ListarAsync(PrestadorServicoSearchFilterDto filtro, CancellationToken ct)
    {
        var page = filtro.Page < 1 ? 1 : filtro.Page;
        var pageSize = filtro.PageSize < 1 ? 10 : Math.Min(filtro.PageSize, 100);

        var query = db.PrestadoresServicos
            .AsNoTracking()
            .Include(x => x.Endereco)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Termo))
        {
            var termo = filtro.Termo.Trim().ToUpperInvariant();
            var matchingSpecs = Enum.GetValues<EspecialidadePrestador>()
                .Where(e => e.ToString().ToUpperInvariant().Contains(termo))
                .ToList();

            query = query.Where(x =>
                x.Nome.ToUpper().Contains(termo) ||
                x.CpfCnpj.Contains(termo) ||
                matchingSpecs.Contains(x.Especialidade)
            );
        }

        if (filtro.Especialidade.HasValue)
        {
            query = query.Where(x => x.Especialidade == filtro.Especialidade.Value);
        }

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == filtro.Ativo.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Guid> CriarAsync(PrestadorServicoCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new ArgumentException("Telefone é obrigatório.", nameof(dto.Telefone));

        Endereco? endereco = null;
        if (dto.Endereco is not null)
        {
            endereco = new Endereco
            {
                Id = Guid.NewGuid(),
                Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit)),
                Logradouro = dto.Endereco.Logradouro,
                Numero = dto.Endereco.Numero,
                Complemento = dto.Endereco.Complemento,
                Bairro = dto.Endereco.Bairro,
                CidadeId = dto.Endereco.CidadeId
            };
        }

        var cleanCpfCnpj = string.Concat(dto.CpfCnpj.Where(char.IsDigit));

        var prest = new PrestadorServico
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Especialidade = dto.Especialidade,
            CpfCnpj = cleanCpfCnpj,
            Telefone = dto.Telefone.Trim(),
            Email = dto.Email?.Trim().ToLowerInvariant(),
            Ativo = dto.Ativo,
            Observacoes = dto.Observacoes?.Trim(),
            Endereco = endereco
        };

        db.PrestadoresServicos.Add(prest);
        await db.SaveChangesAsync(ct);

        return prest.Id;
    }

    public async Task AtualizarAsync(Guid id, PrestadorServicoUpdateDto dto, CancellationToken ct)
    {
        var prest = await db.PrestadoresServicos
            .Include(x => x.Endereco)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Prestador de serviço não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(dto.Nome));

        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new ArgumentException("Telefone é obrigatório.", nameof(dto.Telefone));

        prest.Nome = dto.Nome.Trim();
        prest.Especialidade = dto.Especialidade;
        prest.CpfCnpj = string.Concat(dto.CpfCnpj.Where(char.IsDigit));
        prest.Telefone = dto.Telefone.Trim();
        prest.Email = dto.Email?.Trim().ToLowerInvariant();
        prest.Ativo = dto.Ativo;
        prest.Observacoes = dto.Observacoes?.Trim();

        if (dto.Endereco is not null)
        {
            if (prest.Endereco is null)
            {
                prest.Endereco = new Endereco
                {
                    Id = Guid.NewGuid(),
                    Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit)),
                    Logradouro = dto.Endereco.Logradouro,
                    Numero = dto.Endereco.Numero,
                    Complemento = dto.Endereco.Complemento,
                    Bairro = dto.Endereco.Bairro,
                    CidadeId = dto.Endereco.CidadeId
                };
            }
            else
            {
                prest.Endereco.Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit));
                prest.Endereco.Logradouro = dto.Endereco.Logradouro;
                prest.Endereco.Numero = dto.Endereco.Numero;
                prest.Endereco.Complemento = dto.Endereco.Complemento;
                prest.Endereco.Bairro = dto.Endereco.Bairro;
                prest.Endereco.CidadeId = dto.Endereco.CidadeId;
            }
        }
        else
        {
            prest.EnderecoId = null;
            prest.Endereco = null;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var prest = await db.PrestadoresServicos.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Prestador de serviço não encontrado.");

        db.PrestadoresServicos.Remove(prest);
        await db.SaveChangesAsync(ct);
    }

    private static PrestadorServicoResponseDto Map(PrestadorServico x)
    {
        return new PrestadorServicoResponseDto(
            x.Id,
            x.Nome,
            x.Especialidade,
            x.CpfCnpj,
            x.Telefone,
            x.Email,
            x.Ativo,
            x.Observacoes,
            x.Endereco is null ? null : new EnderecoResponseDto
            {
                Id = x.Endereco.Id,
                CidadeId = x.Endereco.CidadeId,
                Cep = x.Endereco.Cep,
                Logradouro = x.Endereco.Logradouro,
                Numero = x.Endereco.Numero,
                Complemento = x.Endereco.Complemento,
                Bairro = x.Endereco.Bairro
            }
        );
    }
}
