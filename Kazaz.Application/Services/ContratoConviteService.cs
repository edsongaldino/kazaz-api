using Kazaz.Application.Contracts;
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
        var numero = await GerarNumeroContratoAsync(ct);

        var contrato = new Contrato
        {
            Id = Guid.NewGuid(),
            ImovelId = imovelId,
            Tipo = request.Tipo,
            Status = StatusContrato.Rascunho,
            Numero = numero,
            InicioVigencia = DateOnly.FromDateTime(DateTime.UtcNow) // rascunho
        };

        _db.Contratos.Add(contrato);

        var papeis = ObterPapeisContrato(request.Tipo, request.IncluirFiador);

        var baseUrl = _configuration["PublicCadastroBaseUrl"]!
            .TrimEnd('/');

        var expiraEm = DateTime.UtcNow.AddDays(Math.Max(1, request.ExpiraEmDias));

        var links = new List<ConviteLinkResponse>();

        foreach (var papel in papeis)
        {
            var token = GerarTokenSeguro(48);

            var convite = new ConviteCadastroContrato
            {
                Id = Guid.NewGuid(),
                ContratoId = contrato.Id,
                Papel = papel,
                Token = token,
                ExpiraEm = expiraEm
            };

            _db.Set<ConviteCadastroContrato>().Add(convite);

            links.Add(new ConviteLinkResponse(
                papel,
                token,
                $"{baseUrl}/{token}"
            ));
        }

        await _db.SaveChangesAsync(ct);

        return new GerarLinksContratoResponse(
            contrato.Id,
            contrato.Numero,
            links
        );
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
}
