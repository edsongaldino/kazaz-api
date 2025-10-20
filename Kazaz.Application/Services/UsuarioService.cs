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

    public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync() =>
        (await _repo.ObterTodosAsync()).Select(u => new UsuarioDto {
            Id = u.Id, Email = u.Email
        });

    public async Task<UsuarioDto> ObterPorIdAsync(Guid id)
    {
        var u = await _repo.ObterPorIdAsync(id);
        return u == null ? null : new UsuarioDto { Id = u.Id, Email = u.Email };
    }

    public async Task<UsuarioDto> CriarAsync(UsuarioDto dto)
    {
        var u = new Usuario {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Senha = "123456" // Idealmente usar hash e vir da API
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

    public async Task<string> AutenticarAsync(LoginDto login)
    {
        var usuario = await _repo.ObterPorEmailSenhaAsync(login.Email, login.Senha);
        return usuario != null ? _tokenService.GerarToken(usuario) : null;
    }
}