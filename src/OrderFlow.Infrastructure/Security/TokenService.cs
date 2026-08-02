using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public AccessTokenGeradoDto GerarAccessToken(Usuario usuario)
    {
        var agora = DateTimeOffset.UtcNow;
        var expiracao = agora.AddMinutes(_settings.AccessTokenMinutos);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Papel),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _settings.Emissor,
            audience: _settings.Audiencia,
            claims: claims,
            notBefore: agora.UtcDateTime,
            expires: expiracao.UtcDateTime,
            signingCredentials: credenciais);

        return new(new JwtSecurityTokenHandler().WriteToken(jwt), expiracao);
    }

    public string GerarRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string CalcularHashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    public DateTimeOffset ObterExpiracaoRefreshToken() =>
        DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDias);
}
