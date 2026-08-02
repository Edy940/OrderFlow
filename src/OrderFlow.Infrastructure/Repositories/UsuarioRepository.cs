using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly OrderFlowDbContext _context;

    public UsuarioRepository(OrderFlowDbContext context) => _context = context;

    public Task<Usuario?> ObterPorEmailAsync(string email) =>
        _context.Usuarios.SingleOrDefaultAsync(u => u.Email == email);

    public Task<RefreshToken?> ObterRefreshTokenAsync(string tokenHash) =>
        _context.RefreshTokens
            .Include(t => t.Usuario)
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash);

    public async Task AdicionarAsync(Usuario usuario) =>
        await _context.Usuarios.AddAsync(usuario);

    public Task SalvarAlteracoesAsync() => _context.SaveChangesAsync();
}
