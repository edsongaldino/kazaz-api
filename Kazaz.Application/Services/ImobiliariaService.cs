using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Domain.Interfaces;
using Kazaz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class ImobiliariaService : IImobiliariaService
{
    private readonly ApplicationDbContext db;
    private readonly ITenantProvider _tenantProvider;

    public ImobiliariaService(ApplicationDbContext db, ITenantProvider tenantProvider)
    {
        this.db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<ImobiliariaResponseDto> ObterAsync(CancellationToken ct)
    {
        var tenantId = _tenantProvider.ObterImobiliariaId();
        if (!tenantId.HasValue)
        {
            throw new InvalidOperationException("Usuário não está associado a nenhuma imobiliária.");
        }

        var imobiliaria = await db.Imobiliarias
            .Include(x => x.Endereco)
            .FirstOrDefaultAsync(x => x.Id == tenantId.Value, ct);

        if (imobiliaria is null)
        {
            throw new KeyNotFoundException("Imobiliária não encontrada.");
        }

        return Map(imobiliaria);
    }

    public async Task<ImobiliariaResponseDto> SalvarAsync(ImobiliariaUpdateDto dto, CancellationToken ct)
    {
        var tenantId = _tenantProvider.ObterImobiliariaId();
        if (!tenantId.HasValue)
        {
            throw new InvalidOperationException("Usuário não está associado a nenhuma imobiliária.");
        }

        var imobiliaria = await db.Imobiliarias
            .Include(x => x.Endereco)
            .FirstOrDefaultAsync(x => x.Id == tenantId.Value, ct);

        if (imobiliaria is null)
        {
            throw new KeyNotFoundException("Imobiliária não encontrada.");
        }

        UpdateImobiliariaProperties(imobiliaria, dto);

        await db.SaveChangesAsync(ct);
        return Map(imobiliaria);
    }

    // Super Admin methods
    public async Task<(IReadOnlyList<ImobiliariaResponseDto> Items, int Total)> ListarTodasAsync(int page, int pageSize, string? termo, CancellationToken ct)
    {
        var q = db.Imobiliarias.Include(x => x.Endereco).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var cleanTermo = termo.Trim();
            q = q.Where(x => EF.Functions.ILike(x.NomeFantasia, $"%{cleanTermo}%") || EF.Functions.ILike(x.RazaoSocial, $"%{cleanTermo}%"));
        }

        var total = await q.CountAsync(ct);
        var dbItems = await q
            .OrderBy(x => x.NomeFantasia)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = dbItems.Select(Map).ToList();

        return (items, total);
    }

    public async Task<ImobiliariaResponseDto> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var imobiliaria = await db.Imobiliarias.Include(x => x.Endereco).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (imobiliaria == null) throw new KeyNotFoundException("Imobiliária não encontrada.");
        return Map(imobiliaria);
    }

    public async Task<ImobiliariaResponseDto> CriarComAdminAsync(ImobiliariaCriarDto dto, CancellationToken ct)
    {
        var adminEmailClean = dto.AdminEmail.Trim().ToLowerInvariant();
        var emailExists = await db.Usuarios.AnyAsync(u => u.Email.ToLower() == adminEmailClean, ct);
        if (emailExists)
        {
            throw new ArgumentException("Este e-mail de administrador já está cadastrado no sistema.");
        }

        var cleanCnpj = string.Concat(dto.Cnpj.Where(char.IsDigit));
        var cnpjExists = await db.Imobiliarias.AnyAsync(x => x.Cnpj == cleanCnpj, ct);
        if (cnpjExists)
        {
            throw new ArgumentException("Este CNPJ já está cadastrado.");
        }

        var imobiliaria = new Imobiliaria
        {
            Id = Guid.NewGuid(),
            RazaoSocial = dto.RazaoSocial.Trim(),
            NomeFantasia = dto.NomeFantasia.Trim(),
            Cnpj = cleanCnpj,
            Creci = dto.Creci.Trim(),
            DataFundacao = dto.DataFundacao.HasValue ? DateTime.SpecifyKind(dto.DataFundacao.Value, DateTimeKind.Utc) : null,
            LogoUrl = dto.LogoUrl,
            Email = dto.Email?.Trim(),
            Telefone = dto.Telefone?.Trim()
        };

        if (dto.Endereco is not null)
        {
            imobiliaria.Endereco = new Endereco
            {
                Id = Guid.NewGuid(),
                Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit)),
                Logradouro = dto.Endereco.Logradouro,
                Numero = dto.Endereco.Numero,
                Complemento = dto.Endereco.Complemento,
                Bairro = dto.Endereco.Bairro,
                CidadeId = dto.Endereco.CidadeId
            };
        }

        db.Imobiliarias.Add(imobiliaria);

        var perfilImobiliaria = await db.Perfis.FirstOrDefaultAsync(p => p.Nome == "Administrador da Imobiliária", ct);
        if (perfilImobiliaria == null)
        {
            throw new InvalidOperationException("Perfil 'Administrador da Imobiliária' não encontrado. Certifique-se de que os perfis foram seeded.");
        }

        var adminUser = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = dto.AdminNome.Trim(),
            Email = adminEmailClean,
            Senha = Kazaz.SharedKernel.Security.PasswordHasher.Hash(dto.AdminSenha),
            Ativo = true,
            PerfilId = perfilImobiliaria.Id,
            ImobiliariaId = imobiliaria.Id
        };

        db.Usuarios.Add(adminUser);

        await db.SaveChangesAsync(ct);

        return Map(imobiliaria);
    }

    public async Task<ImobiliariaResponseDto> AtualizarPorIdAsync(Guid id, ImobiliariaUpdateDto dto, CancellationToken ct)
    {
        var imobiliaria = await db.Imobiliarias.Include(x => x.Endereco).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (imobiliaria == null) throw new KeyNotFoundException("Imobiliária não encontrada.");

        UpdateImobiliariaProperties(imobiliaria, dto);

        await db.SaveChangesAsync(ct);
        return Map(imobiliaria);
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken ct)
    {
        var imobiliaria = await db.Imobiliarias.FindAsync(new object[] { id }, ct);
        if (imobiliaria == null) return false;

        var users = await db.Usuarios.Where(u => u.ImobiliariaId == id).ToListAsync(ct);
        db.Usuarios.RemoveRange(users);

        db.Imobiliarias.Remove(imobiliaria);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private void UpdateImobiliariaProperties(Imobiliaria imobiliaria, ImobiliariaUpdateDto dto)
    {
        imobiliaria.RazaoSocial = dto.RazaoSocial.Trim();
        imobiliaria.NomeFantasia = dto.NomeFantasia.Trim();
        imobiliaria.Cnpj = string.Concat(dto.Cnpj.Where(char.IsDigit));
        imobiliaria.Creci = dto.Creci.Trim();
        imobiliaria.DataFundacao = dto.DataFundacao.HasValue ? DateTime.SpecifyKind(dto.DataFundacao.Value, DateTimeKind.Utc) : null;
        imobiliaria.LogoUrl = dto.LogoUrl;
        imobiliaria.Email = dto.Email?.Trim();
        imobiliaria.Telefone = dto.Telefone?.Trim();

        if (dto.Endereco is not null)
        {
            if (imobiliaria.Endereco is null)
            {
                imobiliaria.Endereco = new Endereco
                {
                    Id = Guid.NewGuid(),
                    Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit)),
                    Logradouro = dto.Endereco.Logradouro,
                    Numero = dto.Endereco.Numero,
                    Complemento = dto.Endereco.Complemento,
                    Bairro = dto.Endereco.Bairro,
                    CidadeId = dto.Endereco.CidadeId
                };
            }
            else
            {
                imobiliaria.Endereco.Cep = string.Concat(dto.Endereco.Cep.Where(char.IsDigit));
                imobiliaria.Endereco.Logradouro = dto.Endereco.Logradouro;
                imobiliaria.Endereco.Numero = dto.Endereco.Numero;
                imobiliaria.Endereco.Complemento = dto.Endereco.Complemento;
                imobiliaria.Endereco.Bairro = dto.Endereco.Bairro;
                imobiliaria.Endereco.CidadeId = dto.Endereco.CidadeId;
            }
        }
        else
        {
            imobiliaria.EnderecoId = null;
            imobiliaria.Endereco = null;
        }
    }

    private static ImobiliariaResponseDto Map(Imobiliaria x)
    {
        return new ImobiliariaResponseDto(
            x.Id,
            x.RazaoSocial,
            x.NomeFantasia,
            x.Cnpj,
            x.Creci,
            x.DataFundacao,
            x.LogoUrl,
            x.Email,
            x.Telefone,
            x.Endereco is null ? null : new EnderecoResponseDto
            {
                Id = x.Endereco.Id,
                CidadeId = x.Endereco.CidadeId,
                Cep = x.Endereco.Cep,
                Logradouro = x.Endereco.Logradouro,
                Numero = x.Endereco.Numero,
                Complemento = x.Endereco.Complemento,
                Bairro = x.Endereco.Bairro
            }
        );
    }
}
