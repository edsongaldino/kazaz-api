using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class ColaboradorService(
    ApplicationDbContext db,
    IUsuarioService usuarioService
) : IColaboradorService
{
    public async Task<ColaboradorResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var colab = await db.Colaboradores
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Include(x => x.Documentos)
                .ThenInclude(d => d.Documento)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Colaborador não encontrado.");

        return Map(colab);
    }

    public async Task<(IReadOnlyList<ColaboradorResponseDto> Items, int Total)> ListarAsync(ColaboradorSearchFilterDto filtro, CancellationToken ct)
    {
        var page = filtro.Page < 1 ? 1 : filtro.Page;
        var pageSize = filtro.PageSize < 1 ? 10 : Math.Min(filtro.PageSize, 100);

        var query = db.Colaboradores
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Include(x => x.Documentos)
                .ThenInclude(d => d.Documento)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Termo))
        {
            var termo = filtro.Termo.Trim().ToUpperInvariant();
            var matchingCargos = Enum.GetValues<CargoColaborador>()
                .Where(c => c.ToString().ToUpperInvariant().Contains(termo))
                .ToList();

            query = query.Where(x =>
                x.Nome.ToUpper().Contains(termo) ||
                x.Cpf.Contains(termo) ||
                x.Email.ToUpper().Contains(termo) ||
                matchingCargos.Contains(x.Cargo)
            );
        }

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == filtro.Ativo.Value);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => Map(x))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Guid> CriarAsync(ColaboradorCreateDto dto, CancellationToken ct)
    {
        using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            Guid? usuarioId = null;

            if (dto.CriarUsuario)
            {
                if (string.IsNullOrWhiteSpace(dto.Senha))
                    throw new ArgumentException("Senha é obrigatória para criar usuário.");

                if (dto.PerfilId is null || dto.PerfilId == Guid.Empty)
                    throw new ArgumentException("Perfil é obrigatório para criar usuário.");

                var emailLower = dto.Email.Trim().ToLowerInvariant();
                var usuarioExistente = await db.Usuarios.AnyAsync(u => u.Email.ToLower() == emailLower, ct);
                if (usuarioExistente)
                    throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");

                var userDto = new UsuarioDto
                {
                    Nome = dto.Nome.Trim(),
                    Email = emailLower,
                    Senha = dto.Senha,
                    Ativo = dto.Ativo,
                    PerfilId = dto.PerfilId.Value
                };

                var userCreated = await usuarioService.CriarAsync(userDto);
                usuarioId = userCreated.Id;
            }

            var cleanCpf = string.Concat(dto.Cpf.Where(char.IsDigit));
            var colaboradorExistente = await db.Colaboradores.AnyAsync(c => c.Cpf == cleanCpf, ct);
            if (colaboradorExistente)
                throw new InvalidOperationException("Já existe um colaborador cadastrado com este CPF.");

            var colaborador = new Colaborador
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome.Trim(),
                Cpf = cleanCpf,
                Cargo = dto.Cargo,
                Email = dto.Email.Trim().ToLowerInvariant(),
                Telefone = dto.Telefone?.Trim(),
                Ativo = dto.Ativo,
                DataAdmissao = dto.DataAdmissao.HasValue ? DateTime.SpecifyKind(dto.DataAdmissao.Value, DateTimeKind.Utc) : null,
                UsuarioId = usuarioId
            };

            db.Colaboradores.Add(colaborador);

            if (dto.Documentos != null && dto.Documentos.Any())
            {
                foreach (var docInput in dto.Documentos)
                {
                    if (string.IsNullOrWhiteSpace(docInput.Caminho))
                        continue;

                    var doc = new Documento
                    {
                        Id = Guid.NewGuid(),
                        Nome = docInput.DocumentoNome ?? docInput.Nome,
                        Caminho = docInput.Caminho,
                        ContentType = docInput.ContentType,
                        TamanhoBytes = docInput.TamanhoBytes,
                        DataUpload = DateTime.UtcNow
                    };

                    db.Documentos.Add(doc);

                    var colabDoc = new ColaboradorDocumento
                    {
                        Id = Guid.NewGuid(),
                        ColaboradorId = colaborador.Id,
                        Nome = docInput.Nome,
                        DocumentoId = doc.Id,
                        DataAnexo = DateTime.UtcNow
                    };

                    db.ColaboradoresDocumentos.Add(colabDoc);
                }
            }

            await db.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);
            return colaborador.Id;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task AtualizarAsync(Guid id, ColaboradorUpdateDto dto, CancellationToken ct)
    {
        var colaborador = await db.Colaboradores
            .Include(x => x.Usuario)
            .Include(x => x.Documentos)
                .ThenInclude(d => d.Documento)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Colaborador não encontrado.");

        var cleanCpf = string.Concat(dto.Cpf.Where(char.IsDigit));
        if (colaborador.Cpf != cleanCpf)
        {
            var colaboradorExistente = await db.Colaboradores.AnyAsync(c => c.Cpf == cleanCpf && c.Id != id, ct);
            if (colaboradorExistente)
                throw new InvalidOperationException("Já existe outro colaborador cadastrado com este CPF.");
        }

        colaborador.Nome = dto.Nome.Trim();
        colaborador.Cpf = cleanCpf;
        colaborador.Cargo = dto.Cargo;
        colaborador.Email = dto.Email.Trim().ToLowerInvariant();
        colaborador.Telefone = dto.Telefone?.Trim();
        colaborador.Ativo = dto.Ativo;
        colaborador.DataAdmissao = dto.DataAdmissao.HasValue ? DateTime.SpecifyKind(dto.DataAdmissao.Value, DateTimeKind.Utc) : null;

        // Se o colaborador tem um usuário vinculado, podemos sincronizar o Nome e Status Ativo!
        if (colaborador.UsuarioId.HasValue && colaborador.Usuario is not null)
        {
            colaborador.Usuario.Nome = colaborador.Nome;
            colaborador.Usuario.Ativo = colaborador.Ativo;
            colaborador.Usuario.Email = colaborador.Email;
        }

        // Update documents
        var incomingDocs = dto.Documentos ?? new List<ColaboradorDocumentoInputDto>();

        // 1. Remove documents no longer present
        var incomingDocIds = incomingDocs
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToHashSet();

        var docsToRemove = colaborador.Documentos
            .Where(d => !incomingDocIds.Contains(d.Id))
            .ToList();

        foreach (var docToRemove in docsToRemove)
        {
            db.ColaboradoresDocumentos.Remove(docToRemove);
            colaborador.Documentos.Remove(docToRemove);

            var documentEntity = await db.Documentos.FindAsync(new object[] { docToRemove.DocumentoId }, ct);
            if (documentEntity != null)
            {
                db.Documentos.Remove(documentEntity);
            }
        }

        // 2. Add or update incoming documents
        foreach (var docInput in incomingDocs)
        {
            if (docInput.Id.HasValue)
            {
                var existing = colaborador.Documentos.FirstOrDefault(d => d.Id == docInput.Id.Value);
                if (existing != null && existing.Nome != docInput.Nome)
                {
                    existing.Nome = docInput.Nome;
                }
                continue;
            }

            if (string.IsNullOrWhiteSpace(docInput.Caminho))
                continue;

            var doc = new Documento
            {
                Id = Guid.NewGuid(),
                Nome = docInput.DocumentoNome ?? docInput.Nome,
                Caminho = docInput.Caminho,
                ContentType = docInput.ContentType,
                TamanhoBytes = docInput.TamanhoBytes,
                DataUpload = DateTime.UtcNow
            };

            db.Documentos.Add(doc);

            var colabDoc = new ColaboradorDocumento
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaborador.Id,
                Nome = docInput.Nome,
                DocumentoId = doc.Id,
                DataAnexo = DateTime.UtcNow
            };

            db.ColaboradoresDocumentos.Add(colabDoc);
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var colaborador = await db.Colaboradores
            .Include(x => x.Documentos)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Colaborador não encontrado.");

        foreach (var colabDoc in colaborador.Documentos)
        {
            var documentEntity = await db.Documentos.FindAsync(new object[] { colabDoc.DocumentoId }, ct);
            if (documentEntity != null)
            {
                db.Documentos.Remove(documentEntity);
            }
        }

        db.Colaboradores.Remove(colaborador);
        await db.SaveChangesAsync(ct);
    }

    private static ColaboradorResponseDto Map(Colaborador x)
    {
        return new ColaboradorResponseDto(
            x.Id,
            x.Nome,
            x.Cpf,
            x.Cargo,
            x.Email,
            x.Telefone,
            x.Ativo,
            x.DataAdmissao,
            x.UsuarioId,
            x.Usuario?.Email,
            x.Documentos?
                .Select(d => new ColaboradorDocumentoResponseDto(
                    d.Id,
                    d.Nome,
                    d.DocumentoId,
                    d.Documento != null ? d.Documento.Nome : "",
                    d.Documento != null ? d.Documento.Caminho : "",
                    d.Documento?.ContentType,
                    d.Documento?.TamanhoBytes,
                    d.DataAnexo
                ))
                .ToList() ?? new List<ColaboradorDocumentoResponseDto>()
        );
    }
}
