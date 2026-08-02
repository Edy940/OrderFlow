using OrderFlow.Application.DTO;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces;

public interface ITokenService
{
    AccessTokenGeradoDto GerarAccessToken(Usuario usuario);
    string GerarRefreshToken();
    string CalcularHashToken(string token);
    DateTimeOffset ObterExpiracaoRefreshToken();
}

public interface ISenhaHasher
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hashArmazenado);
}
