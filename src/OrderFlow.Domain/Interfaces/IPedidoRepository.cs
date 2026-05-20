
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(Pedido pedido);
        Task<IEnumerable<Pedido?>> ObterTodosAsync();
    }
}
