using Kazaz.Application.Contracts;
using Kazaz.Application.DTOs;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;

namespace Kazaz.Application.Services;

public class ContratoConviteService : IContratoConviteService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public ContratoConviteService(
        ApplicationDbContext db,
        IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<GerarLinksContratoResponse> GerarLinksAsync(
    Guid imovelId,
    GerarLinksContratoRequest request,
    CancellationToken ct)
    {
        if (!PapelEhValidoParaTipo(request.Tipo, request.Papel))
            throw new InvalidOperationException("O papel informado não é válido para o tipo de contrato.");

        var numero = await GerarNumeroContratoAsync(ct);

        var contrato = new Contrato
        {
            Id = Guid.NewGuid(),
            ImovelId = imovelId,
            Tipo = request.Tipo,
            Status = StatusContrato.Rascunho,
            Numero = numero,
            InicioVigencia = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _db.Contratos.Add(contrato);

        var baseUrl = _configuration["PublicUrls:CadastroBaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Config ausente: PublicUrls:CadastroBaseUrl");

        baseUrl = baseUrl.TrimEnd('/');

        var expiraEm = DateTime.UtcNow.AddDays(Math.Max(1, request.ExpiraEmDias));
        var token = GerarTokenSeguro(48);

        var convite = new ConviteCadastroContrato
        {
            Id = Guid.NewGuid(),
            ContratoId = contrato.Id,
            Papel = request.Papel,
            Token = token,
            ExpiraEm = expiraEm
        };

        _db.Set<ConviteCadastroContrato>().Add(convite);

        await _db.SaveChangesAsync(ct);

        return new GerarLinksContratoResponse(
            contrato.Id,
            contrato.Numero,
            new List<ConviteLinkResponse>
            {
            new ConviteLinkResponse(
                request.Papel,
                token,
                $"{baseUrl}/{token}"
            )
            }
        );
    }

    private static bool PapelEhValidoParaTipo(TipoContrato tipo, PapelContrato papel)
    {
        return tipo switch
        {
            TipoContrato.Locacao => papel is PapelContrato.Locatario or PapelContrato.Fiador,
            TipoContrato.Venda => papel is PapelContrato.Comprador or PapelContrato.Vendedor,
            _ => false
        };
    }

    // ============================
    // Helpers
    // ============================

    private static PapelContrato[] ObterPapeisContrato(
        TipoContrato tipo,
        bool incluirFiador)
    {
        return tipo switch
        {
            TipoContrato.Locacao => incluirFiador
                ? new[] { PapelContrato.Locador, PapelContrato.Locatario, PapelContrato.Fiador }
                : new[] { PapelContrato.Locador, PapelContrato.Locatario },

            TipoContrato.Venda or TipoContrato.Compra
                => new[] { PapelContrato.Vendedor, PapelContrato.Comprador },

            _ => Array.Empty<PapelContrato>()
        };
    }

    private static string GerarTokenSeguro(int bytes)
    {
        var data = RandomNumberGenerator.GetBytes(bytes);

        return Convert.ToBase64String(data)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private async Task<string> GerarNumeroContratoAsync(CancellationToken ct)
    {
        var next = await _db.Database
            .SqlQuery<long>($"SELECT nextval('public.contratos_numero_seq') AS \"Value\"")
            .SingleAsync(ct);

        var ano = DateTime.UtcNow.Year;
        return $"{ano}-{next:000000}";
    }

    public async Task<PagedResult<ConviteCadastroListItemResponse>> ListarAsync(
    ListarConvitesCadastroQuery q,
    CancellationToken ct)
    {
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 50 : Math.Min(q.PageSize, 200);

        var baseUrl = _configuration["PublicUrls:CadastroBaseUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Config ausente: PublicUrls:CadastroBaseUrl");

        baseUrl = baseUrl.TrimEnd('/');

        var query = _db.Set<ConviteCadastroContrato>()
            .AsNoTracking()
            .Include(x => x.Contrato)
                .ThenInclude(x => x.Imovel)
            .Include(x => x.Pessoa)
                .ThenInclude(p => p!.PessoaFisica)
            .Include(x => x.Pessoa)
                .ThenInclude(p => p!.PessoaJuridica)
            .Include(x => x.Analises)
            .AsQueryable();

        if (q.ContratoId.HasValue)
            query = query.Where(x => x.ContratoId == q.ContratoId.Value);

        if (q.ImovelId.HasValue)
            query = query.Where(x => x.Contrato.ImovelId == q.ImovelId.Value);

        if (q.Status.HasValue)
            query = query.Where(x => x.Status == q.Status.Value);

        if (q.Papel.HasValue)
            query = query.Where(x => x.Papel == q.Papel.Value);

        if (!string.IsNullOrWhiteSpace(q.Nome))
        {
            var nome = q.Nome.Trim();

            query = query.Where(x =>
                x.Pessoa != null &&
                (
                    x.Pessoa.PessoaFisica != null &&
                    EF.Functions.ILike(x.Pessoa.PessoaFisica.Nome, $"%{nome}%")
                    ||
                    x.Pessoa.PessoaJuridica != null &&
                    (
                        EF.Functions.ILike(x.Pessoa.PessoaJuridica.RazaoSocial, $"%{nome}%") ||
                        EF.Functions.ILike(x.Pessoa.PessoaJuridica.NomeFantasia, $"%{nome}%")
                    )
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(q.Documento))
        {
            var documento = new string(q.Documento.Where(char.IsDigit).ToArray());

            if (!string.IsNullOrWhiteSpace(documento))
            {
                query = query.Where(x =>
                    x.Pessoa != null &&
                    (
                        x.Pessoa.PessoaFisica != null &&
                        EF.Functions.ILike(x.Pessoa.PessoaFisica.Cpf, $"%{documento}%")
                        ||
                        x.Pessoa.PessoaJuridica != null &&
                        EF.Functions.ILike(x.Pessoa.PessoaJuridica.Cnpj, $"%{documento}%")
                    )
                );
            }
        }

        if (!string.IsNullOrWhiteSpace(q.Imovel))
        {
            var termoImovel = q.Imovel.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Contrato.Imovel.Codigo, $"%{termoImovel}%") ||
                EF.Functions.ILike(x.Contrato.Imovel.Titulo, $"%{termoImovel}%")
            );
        }
        if (q.PreenchidoDe.HasValue)
        {
            var de = ToUtcDate(q.PreenchidoDe.Value);
            query = query.Where(x => x.PreenchidoEm >= de);
        }

        if (q.PreenchidoAte.HasValue)
        {
            var ate = ToUtcDate(q.PreenchidoAte.Value).AddDays(1);
            query = query.Where(x => x.PreenchidoEm < ate);
        }

        var total = await query.LongCountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ConviteCadastroListItemResponse(
                x.Id,
                x.ContratoId,
                x.Contrato.Numero,
                x.Contrato.Tipo,
                x.Contrato.Status,
                x.Contrato.ImovelId,

                // Ajuste o campo abaixo conforme sua entidade Imovel
                x.Contrato.Imovel != null
                    ? x.Contrato.Imovel.Titulo
                    : null,

                x.Papel,
                x.Status,
                x.Token,
                $"{baseUrl}/cadastro-publico/{x.Token}",
                x.CriadoEm,
                x.ExpiraEm,
                x.UsadoEm,
                x.PreenchidoEm,
                x.PessoaId,

                x.Pessoa == null
                    ? null
                    : x.Pessoa.PessoaFisica != null
                        ? x.Pessoa.PessoaFisica.Nome
                        : x.Pessoa.PessoaJuridica != null
                            ? x.Pessoa.PessoaJuridica.RazaoSocial
                            : null,

                x.Pessoa == null
                    ? null
                    : x.Pessoa.PessoaFisica != null
                        ? x.Pessoa.PessoaFisica.Cpf
                        : x.Pessoa.PessoaJuridica != null
                            ? x.Pessoa.PessoaJuridica.Cnpj
                            : null,

                x.Analises
                    .OrderByDescending(a => a.CriadoEm)
                    .Select(a => a.Comentario)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);

        return new PagedResult<ConviteCadastroListItemResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }


    public async Task AnalisarConviteAsync(
    Guid conviteId,
    ResultadoAnaliseConvite resultado,
    Guid usuarioId,
    string? comentario,
    CancellationToken ct)
    {
        var convite = await _db.Set<ConviteCadastroContrato>()
            .Include(x => x.Contrato)
            .FirstOrDefaultAsync(x => x.Id == conviteId, ct);

        if (convite is null)
            throw new InvalidOperationException("Convite não encontrado.");

        var analise = new AnaliseConvite
        {
            Id = Guid.NewGuid(),
            ConviteId = convite.Id,
            Resultado = resultado,
            UsuarioId = usuarioId,
            Comentario = comentario,
            CriadoEm = DateTime.UtcNow
        };

        _db.Set<AnaliseConvite>().Add(analise);

        switch (resultado)
        {
            case ResultadoAnaliseConvite.Aprovado:
                convite.Status = StatusConviteCadastro.Aprovado;
                convite.Contrato.Status = StatusContrato.Rascunho;
                break;

            case ResultadoAnaliseConvite.Reprovado:
                convite.Status = StatusConviteCadastro.Reprovado;
                convite.Contrato.Status = StatusContrato.Cancelado;
                break;

            case ResultadoAnaliseConvite.CorrecaoSolicitada:
                convite.Status = StatusConviteCadastro.CorrecaoSolicitada;
                convite.Contrato.Status = StatusContrato.Rascunho;
                break;

            default:
                throw new InvalidOperationException("Resultado da análise inválido.");
        }

        await _db.SaveChangesAsync(ct);
    }


    private static DateTime ToUtcDate(DateTime value)
    {
        return DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);
    }
}
