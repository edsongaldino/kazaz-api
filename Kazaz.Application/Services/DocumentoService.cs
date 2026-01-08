using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Application.Interfaces.Storage;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kazaz.Application.Services;

public class DocumentoService(ApplicationDbContext ctx, IFileStorage storage) : IDocumentoService
{
    public async Task<IReadOnlyList<DocumentoListDto>> ListarPorPessoaAsync(Guid pessoaId, CancellationToken ct)
    {
        return await ctx.Set<PessoaDocumento>().AsNoTracking()
            .Where(a => a.PessoaId == pessoaId)
            .Include(a => a.Documento)
            .Include(a => a.Tipo)
            .OrderBy(a => a.Tipo.Ordem)
            .Select(a => new DocumentoListDto(
                a.DocumentoId,
                a.Documento.Nome,
                a.Documento.Caminho,
                a.Documento.ContentType,
                a.Documento.TamanhoBytes ?? 0,
                a.Documento.DataUpload,
                AlvoDocumento.Pessoa,
                a.PessoaId,
                a.TipoDocumentoId,
                a.Tipo.Nome,
                a.Tipo.Obrigatorio,
                a.Tipo.Ordem,
                a.Observacao
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DocumentoListDto>> ListarPorImovelAsync(Guid imovelId, CancellationToken ct)
    {
        return await ctx.Set<ImovelDocumento>().AsNoTracking()
            .Where(a => a.ImovelId == imovelId)
            .Include(a => a.Documento)
            .Include(a => a.Tipo)
            .OrderBy(a => a.Tipo.Ordem)
            .Select(a => new DocumentoListDto(
                a.DocumentoId,
                a.Documento.Nome,
                a.Documento.Caminho,
                a.Documento.ContentType,
                a.Documento.TamanhoBytes ?? 0,
                a.Documento.DataUpload,
                AlvoDocumento.Imovel,
                a.ImovelId,
                a.TipoDocumentoId,
                a.Tipo.Nome,
                a.Tipo.Obrigatorio,
                a.Tipo.Ordem,
                a.Observacao
            ))
            .ToListAsync(ct);
    }

    public async Task<DocumentoListDto?> ObterAsync(Guid documentoId, CancellationToken ct)
    {
        // tenta achar por pessoa
        var p = await ctx.Set<PessoaDocumento>().AsNoTracking()
            .Include(a => a.Documento).Include(a => a.Tipo)
            .Where(a => a.DocumentoId == documentoId)
            .Select(a => new DocumentoListDto(
                a.DocumentoId, a.Documento.Nome, a.Documento.Caminho, a.Documento.ContentType,
                a.Documento.TamanhoBytes ?? 0, a.Documento.DataUpload,
                AlvoDocumento.Pessoa, a.PessoaId, a.TipoDocumentoId, a.Tipo.Nome, a.Tipo.Obrigatorio, a.Tipo.Ordem, a.Observacao
            )).FirstOrDefaultAsync(ct);

        if (p is not null) return p;

        // tenta achar por imóvel
        var i = await ctx.Set<ImovelDocumento>().AsNoTracking()
            .Include(a => a.Documento).Include(a => a.Tipo)
            .Where(a => a.DocumentoId == documentoId)
            .Select(a => new DocumentoListDto(
                a.DocumentoId, a.Documento.Nome, a.Documento.Caminho, a.Documento.ContentType,
                a.Documento.TamanhoBytes ?? 0, a.Documento.DataUpload,
                AlvoDocumento.Imovel, a.ImovelId, a.TipoDocumentoId, a.Tipo.Nome, a.Tipo.Obrigatorio, a.Tipo.Ordem, a.Observacao
            )).FirstOrDefaultAsync(ct);

        return i;
    }

    public async Task<Guid> CriarAsync(DocumentoCreateDto dto, CancellationToken ct)
    {
        // 1) Valida o tipo e o alvo
        var tipo = await ctx.Set<TipoDocumento>().FirstOrDefaultAsync(t => t.Id == dto.TipoDocumentoId, ct)
            ?? throw new KeyNotFoundException("Tipo de documento não encontrado.");

        switch (dto.Alvo)
        {
            case AlvoDocumento.Pessoa:
                if (tipo.Alvo != AlvoDocumento.Pessoa)
                    throw new InvalidOperationException("Tipo de documento não é do alvo Pessoa.");
                {
                    var ok = await ctx.Set<Pessoa>().AsNoTracking().AnyAsync(p => p.Id == dto.AlvoId, ct);
                    if (!ok) throw new KeyNotFoundException("Pessoa não encontrada.");
                }
                break;

            case AlvoDocumento.Imovel:
                if (tipo.Alvo != AlvoDocumento.Imovel)
                    throw new InvalidOperationException("Tipo de documento não é do alvo Imóvel.");
                {
                    var ok = await ctx.Set<Imovel>().AsNoTracking().AnyAsync(i => i.Id == dto.AlvoId, ct);
                    if (!ok) throw new KeyNotFoundException("Imóvel não encontrado.");
                }
                break;

            default:
                throw new ArgumentException("Alvo inválido.", nameof(dto.Alvo));
        }

        // 2) Cria o arquivo (Documento)
        var doc = new Documento
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome.Trim(),
            Caminho = dto.Caminho.Trim(),
            ContentType = dto.ContentType?.Trim(),
            TamanhoBytes = dto.TamanhoBytes,
            DataUpload = DateTime.UtcNow
        };
        ctx.Add(doc);

        // 3) Cria o vínculo na tabela auxiliar (com UNIQUE por alvo+tipo)
        if (dto.Alvo == AlvoDocumento.Pessoa)
        {
            // UNIQUE (pessoa_id, tipo_documento_id) evita duplicar
            ctx.Add(new PessoaDocumento
            {
                Id = Guid.NewGuid(),
                PessoaId = dto.AlvoId,
                TipoDocumentoId = dto.TipoDocumentoId,
                DocumentoId = doc.Id,
                Observacao = dto.Observacao
            });
        }
        else // Imovel
        {
            // UNIQUE (imovel_id, tipo_documento_id) evita duplicar
            ctx.Add(new ImovelDocumento
            {
                Id = Guid.NewGuid(),
                ImovelId = dto.AlvoId,
                TipoDocumentoId = dto.TipoDocumentoId,
                DocumentoId = doc.Id,
                Observacao = dto.Observacao
            });
        }

        await ctx.SaveChangesAsync(ct);
        return doc.Id;
    }

    public async Task AtualizarAsync(Guid id, DocumentoUpdateDto dto, CancellationToken ct)
    {
        var ent = await ctx.Set<Documento>().FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Documento não encontrado.");

        ent.Nome = dto.Nome.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Caminho)) ent.Caminho = dto.Caminho.Trim();
        ent.ContentType = dto.ContentType?.Trim();
        ent.TamanhoBytes = dto.TamanhoBytes;

        await ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var ent = await ctx.Set<Documento>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (ent is null) return;

        var caminho = ent.Caminho;

        ctx.Remove(ent);
        await ctx.SaveChangesAsync(ct);

        await storage.DeleteAsync(caminho, ct);
    }
}