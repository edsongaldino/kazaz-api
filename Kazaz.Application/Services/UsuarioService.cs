using Kazaz.Application.DTOs;
using Kazaz.Application.Interfaces;
using Kazaz.Domain.Entities;
using Kazaz.Domain.Interfaces;
using Kazaz.SharedKernel.Security;

namespace Kazaz.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly ITokenService _tokenService;

    public UsuarioService(IUsuarioRepository repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<IEnumerable<UsuarioListDto>> ObterTodosAsync() =>
        (await _repo.ObterTodosAsync()).Select(u => new UsuarioListDto
        {
            Id = u.Id, 
            Email = u.Email, 
            Nome = u.Nome, 
            PerfilId = u.PerfilId, 
            Ativo = u.Ativo, 
            PerfilNome = u.Perfil.Nome
        });

    public async Task<UsuarioDto> ObterPorIdAsync(Guid id)
    {
        var u = await _repo.ObterPorIdAsync(id);
        return u == null ? null : new UsuarioDto { Id = u.Id, Email = u.Email };
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioDto dto)
    {
        var u = new Usuario {
            Nome = dto.Nome,
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Senha = PasswordHasher.Hash(dto.Senha),
            Ativo = dto.Ativo,
            PerfilId = dto.PerfilId
        };
        await _repo.CriarAsync(u);
        dto.Id = u.Id;
        return dto;
    }

    public async Task<UsuarioDto> AtualizarAsync(Guid id, UsuarioDto dto)
    {
        var u = await _repo.ObterPorIdAsync(id);
        if (u == null) return null;

        u.Email = dto.Email;
        await _repo.AtualizarAsync(u);
        return dto;
    }

    public async Task<bool> RemoverAsync(Guid id) => await _repo.RemoverAsync(id);

    public async Task<string?> AutenticarAsync(LoginDto login)
    {
        var email = login.Email.Trim().ToLowerInvariant();

        var usuario = await _repo.ObterPorEmailAsync(email);
        if (usuario is null) return null;

        if (!PasswordHasher.Verify(login.Senha, usuario.Senha))
            return null;

        return _tokenService.GerarToken(usuario);
    }

}