using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class RegrasDocumentoCadastroService : IRegrasDocumentoCadastroService
{
    private readonly ApplicationDbContext _ctx;

    public RegrasDocumentoCadastroService(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<RegraDocumentoCadastroResponse>> ListarTodasAsync(CancellationToken ct)
    {
        return await _ctx.Set<RegraDocumentoCadastro>()
            .AsNoTracking()
            .Include(x => x.TipoDocumento)
            .OrderBy(x => x.Ordem)
            .Select(x => new RegraDocumentoCadastroResponse(
                x.Id,
                x.TipoPessoa,
                x.TipoContrato,
                x.PapelContrato,
                x.TipoDocumentoId,
                x.TipoDocumento.Nome,
                x.Obrigatorio,
                x.Ordem,
                x.Multiplicidade,
                x.Rotulo,
                x.Ativo
            ))
            .ToListAsync(ct);
    }

    public async Task<Guid> CriarAsync(CriarRegraDocumentoCadastroRequest req, CancellationToken ct)
    {
        // Valida se o tipo de documento existe
        var docTypeExists = await _ctx.Set<TipoDocumento>().AnyAsync(td => td.Id == req.TipoDocumentoId, ct);
        if (!docTypeExists)
        {
            throw new KeyNotFoundException("Tipo de documento especificado não existe.");
        }

        var ent = new RegraDocumentoCadastro
        {
            Id = Guid.NewGuid(),
            TipoPessoa = req.TipoPessoa,
            TipoContrato = req.TipoContrato,
            PapelContrato = req.PapelContrato,
            TipoDocumentoId = req.TipoDocumentoId,
            Obrigatorio = req.Obrigatorio,
            Ordem = req.Ordem,
            Multiplicidade = req.Multiplicidade,
            Rotulo = req.Rotulo,
            Ativo = req.Ativo
        };

        _ctx.Add(ent);
        await _ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task AtualizarAsync(Guid id, AtualizarRegraDocumentoCadastroRequest req, CancellationToken ct)
    {
        var ent = await _ctx.Set<RegraDocumentoCadastro>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Regra de documento não encontrada.");

        var docTypeExists = await _ctx.Set<TipoDocumento>().AnyAsync(td => td.Id == req.TipoDocumentoId, ct);
        if (!docTypeExists)
        {
            throw new KeyNotFoundException("Tipo de documento especificado não existe.");
        }

        ent.TipoPessoa = req.TipoPessoa;
        ent.TipoContrato = req.TipoContrato;
        ent.PapelContrato = req.PapelContrato;
        ent.TipoDocumentoId = req.TipoDocumentoId;
        ent.Obrigatorio = req.Obrigatorio;
        ent.Ordem = req.Ordem;
        ent.Multiplicidade = req.Multiplicidade;
        ent.Rotulo = req.Rotulo;
        ent.Ativo = req.Ativo;

        await _ctx.SaveChangesAsync(ct);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken ct)
    {
        var ent = await _ctx.Set<RegraDocumentoCadastro>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Regra de documento não encontrada.");

        _ctx.Remove(ent);
        await _ctx.SaveChangesAsync(ct);
    }
}
