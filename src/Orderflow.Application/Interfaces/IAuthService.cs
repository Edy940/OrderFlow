using OrderFlow.Application.DTO;

namespace OrderFlow.Application.Interfaces;

public interface IAuthService
{
    Task<ResultadoAuthDto> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<ResultadoAuthDto> LoginAsync(LoginDto dto);
    Task<ResultadoAuthDto> RenovarAsync(string refreshToken);
    Task<bool> RevogarAsync(string refreshToken);
}
