using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private static readonly List<Pedido> _pedidos = new();

        public Task AdicionarAsync(Pedido pedido)
        {
            _pedidos.Add(pedido);
            return Task.CompletedTask;
        }

        public Task<Pedido> ObterPorIdAsync(Guid id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(pedido);
        }
        public Task SalvarAsync(Pedido pedido)
        {
            _pedidos.Add(pedido);
            return Task.CompletedTask;
        }
    }
}