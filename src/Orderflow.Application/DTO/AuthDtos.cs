namespace OrderFlow.Application.DTO;

public record RegistrarUsuarioDto(string Nome, string Email, string Senha);
public record LoginDto(string Email, string Senha);
public record RenovarTokenDto(string RefreshToken);
public record RevogarTokenDto(string RefreshToken);

public record TokenResponseDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiraEm,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiraEm);

public record ResultadoAuthDto(bool Sucesso, TokenResponseDto? Tokens = null, string? Erro = null);
public record AccessTokenGeradoDto(string Token, DateTimeOffset ExpiraEm);
