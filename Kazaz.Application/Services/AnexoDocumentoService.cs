using Kazaz.Application.DTOs;
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

public class AnexoDocumentoService : IAnexoDocumentoService
{
    private readonly ApplicationDbContext _ctx;
    public AnexoDocumentoService(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<Guid> AnexarParaPessoaAsync(AnexarDocumentoPessoaDto dto, CancellationToken ct)
    {
        // valida tipo alvo
        var tipo = await _ctx.TiposDocumento.FirstOrDefaultAsync(t => t.Id == dto.TipoDocumentoId, ct)
            ?? throw new KeyNotFoundException("Tipo de documento não encontrado.");
        if (tipo.Alvo != AlvoDocumento.Pessoa) throw new InvalidOperationException("Tipo não é de Pessoa.");

        // cria documento
        var doc = new Documento
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Caminho = dto.Caminho.Trim(),
            ContentType = dto.ContentType?.Trim(),
            TamanhoBytes = dto.TamanhoBytes
        };
        _ctx.Documentos.Add(doc);

        // cria vínculo (UNIQUE garante 1 por tipo)
        _ctx.PessoasDocumentos.Add(new PessoaDocumento
        {
            Id = Guid.NewGuid(),
            PessoaId = dto.PessoaId,
            TipoDocumentoId = dto.TipoDocumentoId,
            DocumentoId = doc.Id,
            Observacao = dto.Observacao
        });

        await _ctx.SaveChangesAsync(ct);
        return doc.Id;
    }

    public async Task<Guid> AnexarParaImovelAsync(AnexarDocumentoImovelDto dto, CancellationToken ct)
    {
        var tipo = await _ctx.TiposDocumento.FirstOrDefaultAsync(t => t.Id == dto.TipoDocumentoId, ct)
            ?? throw new KeyNotFoundException("Tipo de documento não encontrado.");
        if (tipo.Alvo != AlvoDocumento.Imovel) throw new InvalidOperationException("Tipo não é de Imóvel.");

        var doc = new Documento
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Caminho = dto.Caminho.Trim(),
            ContentType = dto.ContentType?.Trim(),
            TamanhoBytes = dto.TamanhoBytes
        };
        _ctx.Documentos.Add(doc);

        _ctx.ImoveisDocumentos.Add(new ImovelDocumento
        {
            Id = Guid.NewGuid(),
            ImovelId = dto.ImovelId,
            TipoDocumentoId = dto.TipoDocumentoId,
            DocumentoId = doc.Id,
            Observacao = dto.Observacao
        });

        await _ctx.SaveChangesAsync(ct);
        return doc.Id;
    }

    public async Task RemoverPessoaAsync(Guid pessoaId, Guid tipoDocumentoId, CancellationToken ct)
    {
        var anexo = await _ctx.PessoasDocumentos.FirstOrDefaultAsync(a => a.PessoaId == pessoaId && a.TipoDocumentoId == tipoDocumentoId, ct);
        if (anexo is null) return;
        var doc = await _ctx.Documentos.FindAsync([anexo.DocumentoId], ct);
        _ctx.PessoasDocumentos.Remove(anexo);
        if (doc != null) _ctx.Documentos.Remove(doc);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverImovelAsync(Guid imovelId, Guid tipoDocumentoId, CancellationToken ct)
    {
        var anexo = await _ctx.ImoveisDocumentos.FirstOrDefaultAsync(a => a.ImovelId == imovelId && a.TipoDocumentoId == tipoDocumentoId, ct);
        if (anexo is null) return;
        var doc = await _ctx.Documentos.FindAsync([anexo.DocumentoId], ct);
        _ctx.ImoveisDocumentos.Remove(anexo);
        if (doc != null) _ctx.Documentos.Remove(doc);
        await _ctx.SaveChangesAsync(ct);
    }
}
