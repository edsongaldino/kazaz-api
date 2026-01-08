using Kazaz.Application.DTOs;
using Kazaz.Application.Services.Interfaces;
using Kazaz.Domain;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kazaz.Application.Services;

public class ContratosService : IContratosService
{
    private readonly ApplicationDbContext _db;

    public ContratosService(ApplicationDbContext db) => _db = db;

    public async Task<ContratoResponse> CriarRascunhoAsync(CriarContratoRequest req, CancellationToken ct)
    {
        ValidarRequestBasica(req);

        var tipo = (TipoContrato)req.Tipo;

        // valida entidades
        var imovelExiste = await _db.Imoveis.AnyAsync(x => x.Id == req.ImovelId, ct);
        if (!imovelExiste) throw new InvalidOperationException("Imóvel não encontrado.");

        var pessoaIds = req.Partes.Select(p => p.PessoaId).Distinct().ToList();
        var pessoasCount = await _db.Pessoas.CountAsync(x => pessoaIds.Contains(x.Id), ct);
        if (pessoasCount != pessoaIds.Count) throw new InvalidOperationException("Uma ou mais pessoas informadas não existem.");

        ValidarPartesPorTipo(tipo, req.Partes);

        // gera número (sequência no Postgres) + cria contrato num transaction
        await using var trx = await _db.Database.BeginTransactionAsync(ct);

        var numero = await GerarNumeroContratoAsync(ct);

        var contrato = new Contrato
        {
            Id = Guid.NewGuid(),
            Numero = numero,
            Tipo = tipo,
            Status = StatusContrato.Rascunho,
            ImovelId = req.ImovelId,
            InicioVigencia = req.InicioVigencia,
            FimVigencia = req.FimVigencia,
            CriadoEm = DateTime.UtcNow
        };

        foreach (var parte in req.Partes)
        {
            contrato.Partes.Add(new ContratoParte
            {
                Id = Guid.NewGuid(),
                ContratoId = contrato.Id,
                PessoaId = parte.PessoaId,
                Papel = (PapelContrato)parte.Papel,
                Percentual = parte.Percentual
            });
        }

        _db.Contratos.Add(contrato);
        await _db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return await ObterPorIdAsync(contrato.Id, ct);
    }

    public async Task<ContratoResponse> AtivarAsync(Guid contratoId, CancellationToken ct)
    {
        var contrato = await _db.Contratos
            .Include(x => x.Partes)
            .FirstOrDefaultAsync(x => x.Id == contratoId, ct);

        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        if (contrato.Status == StatusContrato.Ativo) return await ObterPorIdAsync(contratoId, ct);
        if (contrato.Status is StatusContrato.Cancelado or StatusContrato.Encerrado)
            throw new InvalidOperationException("Não é possível ativar um contrato cancelado/encerrado.");

        // valida partes novamente (segurança)
        ValidarPartesPorTipo(contrato.Tipo, contrato.Partes.Select(p =>
            new ContratoParteRequest(p.PessoaId, (int)p.Papel, p.Percentual)).ToList());

        // Regra: só locação tem bloqueio de sobreposição (por enquanto)
        if (contrato.Tipo == TipoContrato.Locacao)
        {
            if (contrato.FimVigencia is null)
                throw new InvalidOperationException("Contrato de locação deve ter fim de vigência.");

            var novoInicio = contrato.InicioVigencia;
            var novoFim = contrato.FimVigencia.Value;

            var existeSobreposicao = await _db.Contratos.AnyAsync(x =>
                x.Id != contrato.Id &&
                x.ImovelId == contrato.ImovelId &&
                x.Tipo == TipoContrato.Locacao &&
                x.Status == StatusContrato.Ativo &&
                // overlap: inicio <= fimExistente && fim >= inicioExistente
                novoInicio <= (x.FimVigencia ?? x.InicioVigencia) &&
                novoFim >= x.InicioVigencia
            , ct);

            if (existeSobreposicao)
                throw new InvalidOperationException("Já existe contrato de locação ATIVO com vigência sobreposta para este imóvel.");
        }

        contrato.Status = StatusContrato.Ativo;
        await _db.SaveChangesAsync(ct);

        return await ObterPorIdAsync(contratoId, ct);
    }

    public async Task<ContratoResponse> CancelarAsync(Guid contratoId, CancellationToken ct)
    {
        var contrato = await _db.Contratos.FirstOrDefaultAsync(x => x.Id == contratoId, ct);
        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        if (contrato.Status == StatusContrato.Encerrado)
            throw new InvalidOperationException("Não é possível cancelar um contrato encerrado.");

        contrato.Status = StatusContrato.Cancelado;
        await _db.SaveChangesAsync(ct);

        return await ObterPorIdAsync(contratoId, ct);
    }

    public async Task<ContratoResponse> EncerrarAsync(Guid contratoId, CancellationToken ct)
    {
        var contrato = await _db.Contratos.FirstOrDefaultAsync(x => x.Id == contratoId, ct);
        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        if (contrato.Status != StatusContrato.Ativo)
            throw new InvalidOperationException("Somente contratos ATIVOS podem ser encerrados.");

        contrato.Status = StatusContrato.Encerrado;
        await _db.SaveChangesAsync(ct);

        return await ObterPorIdAsync(contratoId, ct);
    }

    public async Task<ContratoResponse> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var contrato = await _db.Contratos
            .AsNoTracking()
            .Include(x => x.Partes)
                .ThenInclude(p => p.Pessoa)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        return Map(contrato);
    }

    public async Task<List<ContratoResponse>> ListarAsync(
        Guid? imovelId,
        TipoContrato? tipo,
        StatusContrato? status,
        CancellationToken ct)
    {
        var q = _db.Contratos
            .AsNoTracking()
            .Include(x => x.Partes).ThenInclude(p => p.Pessoa)
            .AsQueryable();

        if (imovelId.HasValue) q = q.Where(x => x.ImovelId == imovelId.Value);
        if (tipo.HasValue) q = q.Where(x => x.Tipo == tipo.Value);
        if (status.HasValue) q = q.Where(x => x.Status == status.Value);

        var list = await q
            .OrderByDescending(x => x.CriadoEm)
            .Take(200)
            .ToListAsync(ct);

        return list.Select(Map).ToList();
    }

    // ===========================
    // Helpers
    // ===========================

    private static void ValidarRequestBasica(CriarContratoRequest req)
    {
        if (req.Partes is null || req.Partes.Count == 0)
            throw new InvalidOperationException("Informe as partes do contrato.");

        if (req.InicioVigencia == default)
            throw new InvalidOperationException("Informe o início da vigência.");

        if (req.FimVigencia.HasValue && req.FimVigencia.Value < req.InicioVigencia)
            throw new InvalidOperationException("Fim da vigência não pode ser menor que o início.");

        // evita papel inválido
        foreach (var p in req.Partes)
        {
            if (!Enum.IsDefined(typeof(PapelContrato), p.Papel))
                throw new InvalidOperationException("Papel de contrato inválido.");
        }
    }

    private static void ValidarPartesPorTipo(TipoContrato tipo, List<ContratoParteRequest> partes)
    {
        // evita duplicadas do mesmo papel pra mesma pessoa
        var duplicadas = partes
            .GroupBy(x => new { x.PessoaId, x.Papel })
            .Any(g => g.Count() > 1);

        if (duplicadas)
            throw new InvalidOperationException("Existe pessoa repetida no mesmo papel do contrato.");

        bool Tem(PapelContrato papel) => partes.Any(x => x.Papel == (int)papel);

        switch (tipo)
        {
            case TipoContrato.Locacao:
                if (!Tem(PapelContrato.Locador))
                    throw new InvalidOperationException("Contrato de locação exige pelo menos 1 LOCADOR.");
                if (!Tem(PapelContrato.Locatario))
                    throw new InvalidOperationException("Contrato de locação exige 1 LOCATÁRIO.");
                break;

            case TipoContrato.Venda:
            case TipoContrato.Compra:
                if (!Tem(PapelContrato.Vendedor))
                    throw new InvalidOperationException("Contrato de venda/compra exige pelo menos 1 VENDEDOR.");
                if (!Tem(PapelContrato.Comprador))
                    throw new InvalidOperationException("Contrato de venda/compra exige pelo menos 1 COMPRADOR.");
                break;

            default:
                throw new InvalidOperationException("Tipo de contrato inválido.");
        }
    }

    private async Task<string> GerarNumeroContratoAsync(CancellationToken ct)
    {
        var next = await _db.Database
            .SqlQuery<long>($"SELECT nextval('public.contratos_numero_seq') AS \"Value\"")
            .SingleAsync(ct);

        var ano = DateTime.UtcNow.Year;
        return $"{ano}-{next:000000}";
    }

    private static ContratoResponse Map(Contrato c)
    {
        return new ContratoResponse(
            c.Id,
            c.Numero,
            (int)c.Tipo,
            (int)c.Status,
            c.ImovelId,
            c.InicioVigencia,
            c.FimVigencia,
            c.CriadoEm,
            c.Partes.Select(p => new ContratoParteResponse(
                p.PessoaId,
                p.Pessoa?.Nome ?? "",
                (int)p.Papel,
                p.Percentual
            )).ToList()
        );
    }
}
