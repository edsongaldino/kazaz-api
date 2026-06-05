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

public class FinanceiroService(ApplicationDbContext db) : IFinanceiroService
{
    public async Task<FinanceiroLancamentoResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var lanc = await db.FinanceiroLancamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Contrato)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lançamento financeiro não encontrado.");

        return Map(lanc);
    }

    public async Task<(IReadOnlyList<FinanceiroLancamentoResponseDto> Items, int Total)> ListarAsync(FinanceiroLancamentoSearchFilterDto filtro, CancellationToken ct)
    {
        var page = filtro.Page < 1 ? 1 : filtro.Page;
        var pageSize = filtro.PageSize < 1 ? 10 : Math.Min(filtro.PageSize, 100);

        var query = db.FinanceiroLancamentos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Contrato)
            .AsQueryable();

        if (filtro.Tipo.HasValue)
        {
            query = query.Where(x => x.Tipo == filtro.Tipo.Value);
        }

        if (filtro.Status.HasValue)
        {
            query = query.Where(x => x.Status == filtro.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            var cat = filtro.Categoria.Trim();
            query = query.Where(x => x.Categoria == cat);
        }

        if (filtro.DataInicio.HasValue)
        {
            query = query.Where(x => x.DataVencimento >= filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            query = query.Where(x => x.DataVencimento <= filtro.DataFim.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.DataVencimento)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Guid> CriarAsync(FinanceiroLancamentoCreateDto dto, CancellationToken ct)
    {
        if (dto.Valor <= 0)
            throw new ArgumentException("O valor do lançamento deve ser maior que zero.", nameof(dto.Valor));

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(dto.Descricao));

        if (string.IsNullOrWhiteSpace(dto.Categoria))
            throw new ArgumentException("Categoria é obrigatória.", nameof(dto.Categoria));

        var lanc = new FinanceiroLancamento
        {
            Id = Guid.NewGuid(),
            Descricao = dto.Descricao.Trim(),
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            Status = dto.Status,
            DataVencimento = DateTime.SpecifyKind(dto.DataVencimento, DateTimeKind.Utc),
            DataPagamento = dto.Status == StatusLancamento.Pago 
                ? (dto.DataPagamento.HasValue ? DateTime.SpecifyKind(dto.DataPagamento.Value, DateTimeKind.Utc) : DateTime.UtcNow) 
                : null,
            Categoria = dto.Categoria.Trim(),
            ClienteId = dto.ClienteId,
            ContratoId = dto.ContratoId
        };

        db.FinanceiroLancamentos.Add(lanc);
        await db.SaveChangesAsync(ct);

        return lanc.Id;
    }

    public async Task AtualizarAsync(Guid id, FinanceiroLancamentoUpdateDto dto, CancellationToken ct)
    {
        var lanc = await db.FinanceiroLancamentos.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lançamento financeiro não encontrado.");

        if (dto.Valor <= 0)
            throw new ArgumentException("O valor do lançamento deve ser maior que zero.", nameof(dto.Valor));

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new ArgumentException("Descrição é obrigatória.", nameof(dto.Descricao));

        if (string.IsNullOrWhiteSpace(dto.Categoria))
            throw new ArgumentException("Categoria é obrigatória.", nameof(dto.Categoria));

        lanc.Descricao = dto.Descricao.Trim();
        lanc.Valor = dto.Valor;
        lanc.Tipo = dto.Tipo;
        lanc.Status = dto.Status;
        lanc.DataVencimento = DateTime.SpecifyKind(dto.DataVencimento, DateTimeKind.Utc);
        lanc.DataPagamento = dto.Status == StatusLancamento.Pago 
            ? (dto.DataPagamento.HasValue 
                ? DateTime.SpecifyKind(dto.DataPagamento.Value, DateTimeKind.Utc) 
                : (lanc.DataPagamento.HasValue ? DateTime.SpecifyKind(lanc.DataPagamento.Value, DateTimeKind.Utc) : DateTime.UtcNow)) 
            : null;
        lanc.Categoria = dto.Categoria.Trim();
        lanc.ClienteId = dto.ClienteId;
        lanc.ContratoId = dto.ContratoId;

        await db.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var lanc = await db.FinanceiroLancamentos.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Lançamento financeiro não encontrado.");

        db.FinanceiroLancamentos.Remove(lanc);
        await db.SaveChangesAsync(ct);
    }

    public async Task<FinanceiroResumoDto> ObterResumoFinanceiroAsync(CancellationToken ct)
    {
        var totalReceberPendente = await db.FinanceiroLancamentos
            .Where(x => x.Tipo == TipoLancamento.Receita && x.Status == StatusLancamento.Pendente)
            .SumAsync(x => (decimal?)x.Valor, ct) ?? 0;

        var totalPagarPendente = await db.FinanceiroLancamentos
            .Where(x => x.Tipo == TipoLancamento.Despesa && x.Status == StatusLancamento.Pendente)
            .SumAsync(x => (decimal?)x.Valor, ct) ?? 0;

        var totalRecebido = await db.FinanceiroLancamentos
            .Where(x => x.Tipo == TipoLancamento.Receita && x.Status == StatusLancamento.Pago)
            .SumAsync(x => (decimal?)x.Valor, ct) ?? 0;

        var totalPago = await db.FinanceiroLancamentos
            .Where(x => x.Tipo == TipoLancamento.Despesa && x.Status == StatusLancamento.Pago)
            .SumAsync(x => (decimal?)x.Valor, ct) ?? 0;

        var saldoLiquido = totalRecebido - totalPago;

        return new FinanceiroResumoDto(
            totalReceberPendente,
            totalPagarPendente,
            totalRecebido,
            totalPago,
            saldoLiquido
        );
    }

    private static FinanceiroLancamentoResponseDto Map(FinanceiroLancamento x)
    {
        string? clienteNome = null;
        if (x.Cliente is not null)
        {
            clienteNome = x.Cliente.PessoaFisica?.Nome ?? x.Cliente.PessoaJuridica?.NomeFantasia ?? x.Cliente.PessoaJuridica?.RazaoSocial ?? "Cliente Sem Nome";
        }

        return new FinanceiroLancamentoResponseDto(
            x.Id,
            x.Descricao,
            x.Valor,
            x.Tipo,
            x.Status,
            x.DataVencimento,
            x.DataPagamento,
            x.Categoria,
            x.ClienteId,
            clienteNome,
            x.ContratoId,
            x.Contrato?.Numero
        );
    }
}
