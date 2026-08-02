using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ISenhaHasher _senhaHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUsuarioRepository usuarios, ISenhaHasher senhaHasher, ITokenService tokenService)
    {
        _usuarios = usuarios;
        _senhaHasher = senhaHasher;
        _tokenService = tokenService;
    }

    public async Task<ResultadoAuthDto> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        var email = NormalizarEmail(dto.Email);
        if (await _usuarios.ObterPorEmailAsync(email) is not null)
            return new(false, Erro: "Já existe um usuário com este e-mail.");

        var usuario = new Usuario(dto.Nome, email, _senhaHasher.GerarHash(dto.Senha));
        await _usuarios.AdicionarAsync(usuario);
        return await CriarSessaoAsync(usuario);
    }

    public async Task<ResultadoAuthDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarios.ObterPorEmailAsync(NormalizarEmail(dto.Email));
        if (usuario is null || !usuario.Ativo || !_senhaHasher.Verificar(dto.Senha, usuario.SenhaHash))
            return new(false, Erro: "E-mail ou senha inválidos.");

        return await CriarSessaoAsync(usuario);
    }

    public async Task<ResultadoAuthDto> RenovarAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return new(false, Erro: "Refresh token inválido.");

        var tokenAtual = await _usuarios.ObterRefreshTokenAsync(_tokenService.CalcularHashToken(refreshToken));
        if (tokenAtual is null || !tokenAtual.EstaAtivo(DateTimeOffset.UtcNow) || !tokenAtual.Usuario.Ativo)
            return new(false, Erro: "Refresh token inválido ou expirado.");

        var novoRefreshToken = _tokenService.GerarRefreshToken();
        var novoHash = _tokenService.CalcularHashToken(novoRefreshToken);
        tokenAtual.Revogar(novoHash);

        var novoTokenPersistido = new RefreshToken(
            tokenAtual.UsuarioId,
            novoHash,
            _tokenService.ObterExpiracaoRefreshToken());
        tokenAtual.Usuario.AdicionarRefreshToken(novoTokenPersistido);

        var accessToken = _tokenService.GerarAccessToken(tokenAtual.Usuario);
        await _usuarios.SalvarAlteracoesAsync();

        return new(true, new(
            accessToken.Token,
            accessToken.ExpiraEm,
            novoRefreshToken,
            novoTokenPersistido.ExpiraEm));
    }

    public async Task<bool> RevogarAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return false;
        var token = await _usuarios.ObterRefreshTokenAsync(_tokenService.CalcularHashToken(refreshToken));
        if (token is null || !token.EstaAtivo(DateTimeOffset.UtcNow)) return false;
        token.Revogar();
        await _usuarios.SalvarAlteracoesAsync();
        return true;
    }

    private async Task<ResultadoAuthDto> CriarSessaoAsync(Usuario usuario)
    {
        var accessToken = _tokenService.GerarAccessToken(usuario);
        var refreshToken = _tokenService.GerarRefreshToken();
        var tokenPersistido = new RefreshToken(
            usuario.Id,
            _tokenService.CalcularHashToken(refreshToken),
            _tokenService.ObterExpiracaoRefreshToken());

        usuario.AdicionarRefreshToken(tokenPersistido);
        await _usuarios.SalvarAlteracoesAsync();

        return new(true, new(
            accessToken.Token,
            accessToken.ExpiraEm,
            refreshToken,
            tokenPersistido.ExpiraEm));
    }

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
