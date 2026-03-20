using Kazaz.Application.DTOs;

namespace Kazaz.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioListDto>> ObterTodosAsync();
    Task<UsuarioDto> ObterPorIdAsync(Guid id);
    Task<UsuarioDto> CriarAsync(UsuarioDto dto);
    Task<UsuarioUpdateDto> AtualizarAsync(Guid id, UsuarioUpdateDto dto);
    Task<bool> RemoverAsync(Guid id);
    Task<string> AutenticarAsync(LoginDto login);
}