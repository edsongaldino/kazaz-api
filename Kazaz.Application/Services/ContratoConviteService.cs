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

    public async Task<PagedResult<ConviteCadastroListItemResponse>> ListarAsync(ListarConvitesCadastroQuery q, CancellationToken ct)
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
            .AsQueryable();

        if (q.ContratoId.HasValue)
            query = query.Where(x => x.ContratoId == q.ContratoId.Value);

        if (q.ImovelId.HasValue)
            query = query.Where(x => x.Contrato.ImovelId == q.ImovelId.Value);

        if (q.Status.HasValue)
            query = query.Where(x => x.Status == q.Status.Value);

        if (q.Papel.HasValue)
            query = query.Where(x => x.Papel == q.Papel.Value);

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
                x.Papel,
                x.Status,
                x.Token,
                $"{baseUrl}/cadastro-publico/{x.Token}",
                x.CriadoEm,
                x.ExpiraEm,
                x.UsadoEm,
                x.PessoaId
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
}
