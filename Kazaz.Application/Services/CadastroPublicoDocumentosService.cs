using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class CadastroPublicoDocumentosService : ICadastroPublicoDocumentosService
{
    private readonly ApplicationDbContext _ctx;

    public CadastroPublicoDocumentosService(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<DocumentosRequeridosResponse> ObterDocumentosRequeridosAsync(string token, CancellationToken ct)
    {
        var convite = await _ctx.Set<ConviteCadastroContrato>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == token, ct)
            ?? throw new KeyNotFoundException("Convite não encontrado.");

        // ✅ aqui pega o enum TipoContrato (não int)
        var contrato = await _ctx.Set<Contrato>()
            .AsNoTracking()
            .Select(c => new { c.Id, c.Tipo })
            .FirstOrDefaultAsync(c => c.Id == convite.ContratoId, ct)
            ?? throw new KeyNotFoundException("Contrato não encontrado.");

        // TipoPessoa derivado
        var tipoPessoaRule = TipoPessoaRule.PF;
        var tipoPessoaLabel = "PF";

        if (convite.PessoaId.HasValue)
        {
            var pessoaInfo = await _ctx.Set<Pessoa>()
                .AsNoTracking()
                .Where(p => p.Id == convite.PessoaId.Value)
                .Select(p => new
                {
                    TemPF = p.PessoaFisica != null,
                    TemPJ = p.PessoaJuridica != null
                })
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException("Pessoa vinculada ao convite não foi encontrada.");

            if (pessoaInfo.TemPJ)
            {
                tipoPessoaRule = TipoPessoaRule.PJ;
                tipoPessoaLabel = "PJ";
            }
        }

        // ✅ agora switch no enum TipoContrato
        var tipoContratoRule = contrato.Tipo switch
        {
            TipoContrato.Locacao => TipoContratoRule.Locacao,
            TipoContrato.Compra => TipoContratoRule.Compra,
            TipoContrato.Venda => TipoContratoRule.Venda,
            _ => TipoContratoRule.Any
        };

        var papelRule = convite.Papel switch
        {
            PapelContrato.Locador => PapelContratoRule.Locador,
            PapelContrato.Locatario => PapelContratoRule.Locatario,
            PapelContrato.Fiador => PapelContratoRule.Fiador,
            PapelContrato.Vendedor => PapelContratoRule.Vendedor,
            PapelContrato.Comprador => PapelContratoRule.Comprador,
            _ => PapelContratoRule.Any
        };


        var regras = await _ctx.Set<RegraDocumentoCadastro>()
            .AsNoTracking()
            .Include(x => x.TipoDocumento)
            .Where(x => x.Ativo)
            .Where(x =>
                (x.TipoPessoa == TipoPessoaRule.Any || x.TipoPessoa == tipoPessoaRule) &&
                (x.TipoContrato == TipoContratoRule.Any || x.TipoContrato == tipoContratoRule) &&
                (x.PapelContrato == PapelContratoRule.Any || x.PapelContrato == papelRule)
            )
            .OrderBy(x => x.Ordem)
            .ToListAsync(ct);

        var itens = new List<DocumentoRequeridoDto>();

        foreach (var r in regras)
        {
            var nomeBase = string.IsNullOrWhiteSpace(r.Rotulo) ? r.TipoDocumento.Nome : r.Rotulo!;
            var mult = r.Multiplicidade <= 0 ? 1 : r.Multiplicidade;

            if (mult == 1)
            {
                itens.Add(new DocumentoRequeridoDto
                {
                    TipoDocumentoId = r.TipoDocumentoId,
                    Nome = nomeBase,
                    Obrigatorio = r.Obrigatorio,
                    Ordem = r.Ordem,
                    MultiplicidadeIndex = null
                });
                continue;
            }

            for (int i = 1; i <= mult; i++)
            {
                itens.Add(new DocumentoRequeridoDto
                {
                    TipoDocumentoId = r.TipoDocumentoId,
                    Nome = $"{nomeBase} ({i})",
                    Obrigatorio = r.Obrigatorio,
                    Ordem = r.Ordem,
                    MultiplicidadeIndex = i
                });
            }
        }

        return new DocumentosRequeridosResponse
        {
            ContratoId = convite.ContratoId,
            PessoaId = convite.PessoaId,

            // ✅ se seu response usa int, converte aqui
            TipoContrato = (int)contrato.Tipo,
            PapelContrato = (int)convite.Papel,
            TipoPessoa = tipoPessoaLabel,

            Itens = itens
        };
    }
}
