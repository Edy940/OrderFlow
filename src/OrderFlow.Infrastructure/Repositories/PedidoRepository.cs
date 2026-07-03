using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;

namespace OrderFlow.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly OrderFlowDbContext _context;

        public PedidoRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pedido>> ObterTodosAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ToListAsync();
        }

        public async Task<Pedido?> ObterPorIdAsync(Guid id)
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                 .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}