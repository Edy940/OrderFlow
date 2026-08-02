using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<RefreshToken?> ObterRefreshTokenAsync(string tokenHash);
    Task AdicionarAsync(Usuario usuario);
    Task SalvarAlteracoesAsync();
}
