using Kazaz.Application.DTOs;
using Kazaz.Application.Services.Interfaces;
using Kazaz.Domain;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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
            .Include(x => x.Imovel)
                .ThenInclude(i => i.TipoImovel)
            .Include(x => x.Partes)
                .ThenInclude(p => p.Pessoa)
                    .ThenInclude(p => p.PessoaFisica)
            .Include(x => x.Partes)
                .ThenInclude(p => p.Pessoa)
                    .ThenInclude(p => p.PessoaJuridica)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        return Map(contrato);
    }

    public async Task<ContratoResponse> AtualizarAsync(Guid id, AtualizarContratoRequest req, CancellationToken ct)
    {
        var contrato = await _db.Contratos
            .Include(x => x.Partes)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (contrato is null) throw new InvalidOperationException("Contrato não encontrado.");

        contrato.ImovelId = req.ImovelId;
        contrato.InicioVigencia = req.InicioVigencia;
        contrato.FimVigencia = req.FimVigencia;
        contrato.Status = (StatusContrato)req.Status;

        // Remove partes anteriores e insere as novas
        _db.ContratoPartes.RemoveRange(contrato.Partes);

        contrato.Partes = req.Partes.Select(p => new ContratoParte
        {
            PessoaId = p.PessoaId,
            Papel = (PapelContrato)p.Papel,
            Percentual = p.Percentual
        }).ToList();

        await _db.SaveChangesAsync(ct);

        return await ObterPorIdAsync(id, ct);
    }

    public async Task<PagedResult<ContratoResponse>> ListarAsync(
    ListarContratosQuery query,
    CancellationToken ct)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var q = _db.Contratos
            .AsNoTracking()
            .Include(x => x.Imovel)
                .ThenInclude(i => i.TipoImovel)
            .Include(x => x.Partes)
                .ThenInclude(p => p.Pessoa)
                    .ThenInclude(p => p.PessoaFisica)
            .Include(x => x.Partes)
                .ThenInclude(p => p.Pessoa)
                    .ThenInclude(p => p.PessoaJuridica)
            .Where(x => x.Status != StatusContrato.Rascunho)
            .AsQueryable();

        if (query.ImovelId.HasValue)
            q = q.Where(x => x.ImovelId == query.ImovelId.Value);

        if (query.TipoImovelId.HasValue)
            q = q.Where(x => x.Imovel.TipoImovelId == query.TipoImovelId.Value);

        if (query.Tipo.HasValue)
            q = q.Where(x => x.Tipo == query.Tipo.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Contrato))
        {
            var contrato = query.Contrato.Trim();
            q = q.Where(x => EF.Functions.ILike(x.Numero, $"%{contrato}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.Imovel))
        {
            var imovel = query.Imovel.Trim();

            q = q.Where(x =>
                EF.Functions.ILike(x.Imovel.Codigo, $"%{imovel}%") ||
                EF.Functions.ILike(x.Imovel.Titulo, $"%{imovel}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.DocumentoParte))
        {
            var documento = new string(query.DocumentoParte.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrWhiteSpace(documento))
            {
                q = q.Where(x => x.Partes.Any(p =>
                    (
                        p.Pessoa.PessoaFisica != null &&
                        EF.Functions.ILike(p.Pessoa.PessoaFisica.Cpf, $"%{documento}%")
                    )
                    ||
                    (
                        p.Pessoa.PessoaJuridica != null &&
                        EF.Functions.ILike(p.Pessoa.PessoaJuridica.Cnpj, $"%{documento}%")
                    )
                ));
            }
        }

        if (query.VigenciaDe.HasValue)
            q = q.Where(x => x.InicioVigencia >= query.VigenciaDe.Value);

        if (query.VigenciaAte.HasValue)
            q = q.Where(x => x.InicioVigencia <= query.VigenciaAte.Value);

        var total = await q.LongCountAsync(ct);

        var list = await q
            .OrderByDescending(x => x.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ContratoResponse>
        {
            Items = list.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            Total = total
        };
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
            c.Imovel?.Codigo,
            c.Imovel?.Titulo,
            c.Imovel?.TipoImovel?.Nome,
            c.InicioVigencia,
            c.FimVigencia,
            c.CriadoEm,
            c.Partes.Select(p => new ContratoParteResponse(
                p.PessoaId,
                p.Pessoa != null
                    ? p.Pessoa.PessoaFisica != null
                        ? p.Pessoa.PessoaFisica.Nome
                        : p.Pessoa.PessoaJuridica != null
                            ? p.Pessoa.PessoaJuridica.RazaoSocial
                            : "-"
                    : "-",
                (int)p.Papel,
                p.Percentual
            )).ToList()
        );
    }

    public async Task<ContratoChecklistEntradaResponse> ObterChecklistEntradaAsync(Guid contratoId, CancellationToken ct)
    {
        var e = await _db.ContratoChecklistEntrada
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (e is null)
        {
            return new ContratoChecklistEntradaResponse(
                contratoId, null, null, null, null, null, null, null, null, null, null, null, null, null, null
            );
        }

        return MapEntrada(e);
    }

    public async Task<ContratoChecklistEntradaResponse> SalvarChecklistEntradaAsync(Guid contratoId, SalvarChecklistEntradaRequest req, CancellationToken ct)
    {
        var contratoExiste = await _db.Contratos.AnyAsync(x => x.Id == contratoId, ct);
        if (!contratoExiste) throw new InvalidOperationException("Contrato não encontrado.");

        var e = await _db.ContratoChecklistEntrada
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (e is null)
        {
            e = new ContratoChecklistEntrada { ContratoId = contratoId };
            _db.ContratoChecklistEntrada.Add(e);
        }

        e.AssinadoEm = req.AssinadoEm;
        e.SeguroIncendio = req.SeguroIncendio;
        e.Chaves = req.Chaves;
        e.Energia = req.Energia;
        e.Agua = req.Agua;
        e.Gas = req.Gas;
        e.Condominio = req.Condominio;
        e.IptuGaragem = req.IptuGaragem;
        e.Iptu = req.Iptu;
        e.VistoriaEntradaEm = req.VistoriaEntradaEm;
        e.Manutencao = req.Manutencao;
        e.ObservacoesFinais = req.ObservacoesFinais;
        e.BonusLocacao = req.BonusLocacao;
        e.DataPagamentoBonus = req.DataPagamentoBonus;

        await _db.SaveChangesAsync(ct);
        return MapEntrada(e);
    }

    public async Task<ContratoChecklistSaidaResponse> ObterChecklistSaidaAsync(Guid contratoId, CancellationToken ct)
    {
        var s = await _db.ContratoChecklistSaida
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (s is null)
        {
            return new ContratoChecklistSaidaResponse(
                contratoId, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
            );
        }

        return MapSaida(s);
    }

    public async Task<ContratoChecklistSaidaResponse> SalvarChecklistSaidaAsync(Guid contratoId, SalvarChecklistSaidaRequest req, CancellationToken ct)
    {
        var contratoExiste = await _db.Contratos.AnyAsync(x => x.Id == contratoId, ct);
        if (!contratoExiste) throw new InvalidOperationException("Contrato não encontrado.");

        var s = await _db.ContratoChecklistSaida
            .FirstOrDefaultAsync(x => x.ContratoId == contratoId, ct);

        if (s is null)
        {
            s = new ContratoChecklistSaida { ContratoId = contratoId };
            _db.ContratoChecklistSaida.Add(s);
        }

        s.MotivoSaida = req.MotivoSaida;
        s.Aluguel = req.Aluguel;
        s.MultaContratual = req.MultaContratual;
        s.AvisoSaidaEm = req.AvisoSaidaEm;
        s.Chaves = req.Chaves;
        s.AvisoProprietario = req.AvisoProprietario;
        s.Energia = req.Energia;
        s.Gas = req.Gas;
        s.Agua = req.Agua;
        s.Condominio = req.Condominio;
        s.Iptu = req.Iptu;
        s.VistoriaSaidaEm = req.VistoriaSaidaEm;
        s.PinturaManutencao = req.PinturaManutencao;
        s.ReativarImovelNoSite = req.ReativarImovelNoSite;
        s.CancelamentoSeguroFianca = req.CancelamentoSeguroFianca;

        await _db.SaveChangesAsync(ct);
        return MapSaida(s);
    }

    private static ContratoChecklistEntradaResponse MapEntrada(ContratoChecklistEntrada e)
    {
        return new ContratoChecklistEntradaResponse(
            e.ContratoId,
            e.AssinadoEm,
            e.SeguroIncendio,
            e.Chaves,
            e.Energia,
            e.Agua,
            e.Gas,
            e.Condominio,
            e.IptuGaragem,
            e.Iptu,
            e.VistoriaEntradaEm,
            e.Manutencao,
            e.ObservacoesFinais,
            e.BonusLocacao,
            e.DataPagamentoBonus
        );
    }

    private static ContratoChecklistSaidaResponse MapSaida(ContratoChecklistSaida s)
    {
        return new ContratoChecklistSaidaResponse(
            s.ContratoId,
            s.MotivoSaida,
            s.Aluguel,
            s.MultaContratual,
            s.AvisoSaidaEm,
            s.Chaves,
            s.AvisoProprietario,
            s.Energia,
            s.Gas,
            s.Agua,
            s.Condominio,
            s.Iptu,
            s.VistoriaSaidaEm,
            s.PinturaManutencao,
            s.ReativarImovelNoSite,
            s.CancelamentoSeguroFianca
        );
    }
}
