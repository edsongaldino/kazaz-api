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

public class TipoDocumentoService : ITipoDocumentoService
{
    private readonly ApplicationDbContext _ctx;

    public TipoDocumentoService(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Guid> CriarAsync(TipoDocumentoCreateDto dto, CancellationToken ct)
    {
        var ent = new TipoDocumento
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Alvo = dto.Alvo,
            Obrigatorio = dto.Obrigatorio,
            Ordem = dto.Ordem,
            Descricao = dto.Descricao?.Trim(),
            Ativo = true
        };

        _ctx.Add(ent);
        await _ctx.SaveChangesAsync(ct);
        return ent.Id;
    }

    public async Task UpdateAsync(Guid id, TipoDocumentoUpdateDto dto, CancellationToken ct)
    {
        await AtualizarAsync(id, dto, ct);
    }

    public async Task AtualizarAsync(Guid id, TipoDocumentoUpdateDto dto, CancellationToken ct)
    {
        var ent = await _ctx.Set<TipoDocumento>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Tipo de documento não encontrado.");

        ent.Nome = dto.Nome.Trim();
        ent.Obrigatorio = dto.Obrigatorio;
        ent.Ordem = dto.Ordem;
        ent.Ativo = dto.Ativo;
        ent.Descricao = dto.Descricao?.Trim();

        await _ctx.SaveChangesAsync(ct);
    }

    public async Task<List<TipoDocumento>> ListarPorAlvoAsync(AlvoDocumento alvo, bool? somenteObrigatorios, CancellationToken ct)
    {
        var query = _ctx.Set<TipoDocumento>().AsNoTracking()
            .Where(x => x.Alvo == alvo && x.Ativo);

        if (somenteObrigatorios.HasValue && somenteObrigatorios.Value)
        {
            query = query.Where(x => x.Obrigatorio);
        }

        return await query.OrderBy(x => x.Ordem).ToListAsync(ct);
    }

    public async Task<List<TipoDocumento>> ListarTodosAsync(CancellationToken ct)
    {
        return await _ctx.Set<TipoDocumento>().AsNoTracking()
            .OrderBy(x => x.Alvo)
            .ThenBy(x => x.Ordem)
            .ToListAsync(ct);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken ct)
    {
        // Verifica se é referenciado por alguma regra de documento de cadastro
        var isReferencedByRules = await _ctx.Set<RegraDocumentoCadastro>().AnyAsync(x => x.TipoDocumentoId == id, ct);
        if (isReferencedByRules)
        {
            throw new InvalidOperationException("Este tipo de documento não pode ser excluído pois está associado a uma ou mais regras de documentos obrigatórios.");
        }

        var ent = await _ctx.Set<TipoDocumento>()
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Tipo de documento não encontrado.");

        _ctx.Remove(ent);
        await _ctx.SaveChangesAsync(ct);
    }
}
