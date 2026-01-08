using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioListDto>> ObterTodosAsync();
    Task<UsuarioDto> ObterPorIdAsync(Guid id);
    Task<UsuarioDto> CriarAsync(UsuarioDto dto);
    Task<UsuarioDto> AtualizarAsync(Guid id, UsuarioDto dto);
    Task<bool> RemoverAsync(Guid id);
    Task<string> AutenticarAsync(LoginDto login);
}