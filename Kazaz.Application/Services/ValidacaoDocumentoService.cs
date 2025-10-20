using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class ValidacaoDocumentoService : IValidacaoDocumentoService
{
    private readonly ApplicationDbContext _ctx;
    public ValidacaoDocumentoService(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<List<TipoDocumento>> FaltantesPessoaAsync(Guid pessoaId, CancellationToken ct)
    {
        var obrig = _ctx.TiposDocumento.AsNoTracking()
            .Where(t => t.Alvo == AlvoDocumento.Pessoa && t.Ativo && t.Obrigatorio);

        var existentes = _ctx.PessoasDocumentos.AsNoTracking()
            .Where(a => a.PessoaId == pessoaId)
            .Select(a => a.TipoDocumentoId);

        return await obrig.Where(t => !existentes.Contains(t.Id))
                          .OrderBy(t => t.Ordem)
                          .ToListAsync(ct);
    }

    public async Task<List<TipoDocumento>> FaltantesImovelAsync(Guid imovelId, CancellationToken ct)
    {
        var obrig = _ctx.TiposDocumento.AsNoTracking()
            .Where(t => t.Alvo == AlvoDocumento.Imovel && t.Ativo && t.Obrigatorio);

        var existentes = _ctx.ImoveisDocumentos.AsNoTracking()
            .Where(a => a.ImovelId == imovelId)
            .Select(a => a.TipoDocumentoId);

        return await obrig.Where(t => !existentes.Contains(t.Id))
                          .OrderBy(t => t.Ordem)
                          .ToListAsync(ct);
    }
}