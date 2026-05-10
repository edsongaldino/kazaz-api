using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Domain.Interfaces;
using Kazaz.Infrastructure.Data;
using Kazaz.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;

namespace Kazaz.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext ctx;
    private readonly IUsuarioRepository _repo;
    private readonly ITokenService _tokenService;

    public UsuarioService(
        ApplicationDbContext ctx,
        IUsuarioRepository repo,
        ITokenService tokenService)
    {
        this.ctx = ctx;
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<(IReadOnlyList<UsuarioListDto> Items, int Total)> ListarAsync(
        UsuarioFiltroDto filtro,
        CancellationToken ct)
    {
        var page = filtro.Page < 1 ? 1 : filtro.Page;
        var pageSize = filtro.PageSize < 1 ? 10 : Math.Min(filtro.PageSize, 100);

        var q = ctx.Set<Usuario>()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Termo))
        {
            var termo = filtro.Termo.Trim();

            q = q.Where(u =>
                EF.Functions.ILike(u.Nome, $"%{termo}%") ||
                EF.Functions.ILike(u.Email, $"%{termo}%")
            );
        }

        if (filtro.PerfilId.HasValue)
        {
            q = q.Where(u => u.PerfilId == filtro.PerfilId.Value);
        }

        if (filtro.Ativo.HasValue)
        {
            q = q.Where(u => u.Ativo == filtro.Ativo.Value);
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsuarioListDto
            {
                Id = u.Id,
                Email = u.Email,
                Nome = u.Nome,
                PerfilId = u.PerfilId,
                Ativo = u.Ativo,
                PerfilNome = u.Perfil.Nome
            })
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IEnumerable<UsuarioListDto>> ObterTodosAsync()
    {
        return await ctx.Set<Usuario>()
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioListDto
            {
                Id = u.Id,
                Email = u.Email,
                Nome = u.Nome,
                PerfilId = u.PerfilId,
                Ativo = u.Ativo,
                PerfilNome = u.Perfil.Nome
            })
            .ToListAsync();
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(Guid id)
    {
        var u = await _repo.ObterPorIdAsync(id);

        return u == null
            ? null
            : new UsuarioDto
            {
                Id = u.Id,
                Email = u.Email,
                Nome = u.Nome,
                Ativo = u.Ativo,
                PerfilId = u.PerfilId
            };
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioDto dto)
    {
        var u = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = PasswordHasher.Hash(dto.Senha),
            Ativo = dto.Ativo,
            PerfilId = dto.PerfilId
        };

        await _repo.CriarAsync(u);

        dto.Id = u.Id;
        return dto;
    }

    public async Task<UsuarioUpdateDto?> AtualizarAsync(Guid id, UsuarioUpdateDto dto)
    {
        var u = await _repo.ObterPorIdAsync(id);
        if (u == null) return null;

        u.Email = dto.Email;
        u.Nome = dto.Nome;
        u.Ativo = dto.Ativo;
        u.PerfilId = dto.PerfilId;

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            u.Senha = PasswordHasher.Hash(dto.Senha);
        }

        await _repo.AtualizarAsync(u);

        return dto;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        return await _repo.RemoverAsync(id);
    }

    public async Task<LoginResponseDto?> AutenticarAsync(LoginDto login)
    {
        var email = login.Email.Trim().ToLowerInvariant();

        var usuario = await _repo.ObterPorEmailAsync(email);
        if (usuario is null) return null;

        if (!PasswordHasher.Verify(login.Senha, usuario.Senha))
            return null;

        var token = _tokenService.GerarToken(usuario);

        return new LoginResponseDto
        {
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            PerfilId = usuario.PerfilId,
            PerfilNome = usuario.Perfil?.Nome,
            Token = token
        };
    }
}