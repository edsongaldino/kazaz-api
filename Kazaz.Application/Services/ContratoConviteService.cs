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

    /// <summary>
    /// Gera convites de cadastro vinculados a um contrato de locacao ou venda.
    ///
    /// Logica para Locacao:
    ///   - FormaGarantia = Fiador    => cria convite Locatario + convite Fiador
    ///   - FormaGarantia = SeguroFianca => cria apenas convite Locatario
    ///
    /// Logica para Venda:
    ///   - Cria convite Comprador (o Vendedor e o proprietario, vinculado ao imovel)
    ///
    /// Se request.ContratoId for informado, adiciona convites ao contrato existente.
    /// Caso contrario, cria um novo contrato.
    ///
    /// O Locador (proprietario) NAO e mais gerado como convite; ele e vinculado ao imovel.
    /// </summary>
    public async Task<GerarLinksContratoResponse> GerarLinksAsync(
        Guid imovelId,
        GerarLinksContratoRequest request,
        CancellationToken ct)
    {
        // --- Validacoes ---
        if (request.Tipo == TipoContrato.Locacao && request.FormaGarantia is null)
            throw new InvalidOperationException("Informe a forma de garantia para contratos de locacao (Fiador ou Seguro Fianca).");

        var imovelExiste = await _db.Imoveis.AnyAsync(x => x.Id == imovelId, ct);
        if (!imovelExiste)
            throw new InvalidOperationException("Imovel nao encontrado.");

        var baseUrl = _configuration["PublicUrls:CadastroBaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Config ausente: PublicUrls:CadastroBaseUrl");
        baseUrl = baseUrl.TrimEnd('/');

        var expiraEm = DateTime.UtcNow.AddDays(Math.Max(1, request.ExpiraEmDias));

        // --- Obter ou criar contrato ---
        Contrato contrato;

        if (request.ContratoId.HasValue)
        {
            contrato = await _db.Contratos
                .FirstOrDefaultAsync(x => x.Id == request.ContratoId.Value && x.ImovelId == imovelId, ct)
                ?? throw new InvalidOperationException("Contrato nao encontrado para este imovel.");
        }
        else
        {
            var numero = await GerarNumeroContratoAsync(ct);

            contrato = new Contrato
            {
                Id = Guid.NewGuid(),
                ImovelId = imovelId,
                Tipo = request.Tipo,
                Status = StatusContrato.EmAnalise,
                Numero = numero,
                InicioVigencia = DateOnly.FromDateTime(DateTime.UtcNow),
                FormaGarantia = request.Tipo == TipoContrato.Locacao ? request.FormaGarantia : null,
                AdministradoPeloProprietario = request.AdministradoPeloProprietario
            };

            _db.Contratos.Add(contrato);
        }

        // --- Determinar papeis dos convites a criar ---
        var papeis = DeterminarPapeis(request.Tipo, request.FormaGarantia);

        // --- Criar convites ---
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
                $"{baseUrl}/cadastro-publico/{token}"
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

    /// <summary>
    /// Determina quais papeis de convite devem ser gerados conforme tipo e forma de garantia.
    /// O proprietário é incluído na geração de convites como PapelContrato.Proprietario.
    /// </summary>
    private static IReadOnlyList<PapelContrato> DeterminarPapeis(
        TipoContrato tipo,
        FormaGarantiaLocacao? formaGarantia)
    {
        return tipo switch
        {
            TipoContrato.Locacao => formaGarantia == FormaGarantiaLocacao.Fiador
                ? new[] { PapelContrato.Proprietario, PapelContrato.Locatario, PapelContrato.Fiador }
                : new[] { PapelContrato.Proprietario, PapelContrato.Locatario },

            TipoContrato.Venda or TipoContrato.Compra
                => new[] { PapelContrato.Proprietario, PapelContrato.Comprador },

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
            throw new InvalidOperationException("Convite nao encontrado.");

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
                if (convite.PessoaId.HasValue)
                {
                    if (convite.Papel == PapelContrato.Proprietario)
                    {
                        var imovelId = convite.Contrato.ImovelId;
                        var proprietarioJaExiste = await _db.Set<ImovelProprietario>()
                            .AnyAsync(x => x.ImovelId == imovelId && x.PessoaId == convite.PessoaId.Value && x.Ativo, ct);

                        if (!proprietarioJaExiste)
                        {
                            _db.Set<ImovelProprietario>().Add(new ImovelProprietario
                            {
                                Id = Guid.NewGuid(),
                                ImovelId = imovelId,
                                PessoaId = convite.PessoaId.Value,
                                Ativo = true,
                                CriadoEm = DateTime.UtcNow
                            });
                        }
                    }
                    else
                    {
                        var jaExiste = await _db.Set<ContratoParte>()
                            .AnyAsync(x => x.ContratoId == convite.ContratoId && x.PessoaId == convite.PessoaId.Value, ct);
                        if (!jaExiste)
                        {
                            _db.Set<ContratoParte>().Add(new ContratoParte
                            {
                                Id = Guid.NewGuid(),
                                ContratoId = convite.ContratoId,
                                PessoaId = convite.PessoaId.Value,
                                Papel = convite.Papel
                            });
                        }
                    }
                }
                break;

            case ResultadoAnaliseConvite.Reprovado:
                convite.Status = StatusConviteCadastro.Reprovado;
                convite.Contrato.Status = StatusContrato.Cancelado;
                break;

            case ResultadoAnaliseConvite.CorrecaoSolicitada:
                convite.Status = StatusConviteCadastro.CorrecaoSolicitada;
                break;

            default:
                throw new InvalidOperationException("Resultado da analise invalido.");
        }

        await _db.SaveChangesAsync(ct);
    }


    private static DateTime ToUtcDate(DateTime value)
    {
        return DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);
    }
}
