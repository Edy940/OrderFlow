using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task AdicionarAsync(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterTodosAsync(); //Diferente de QueryAble, pois não é uma consulta, mas sim um processamento em memória, não persistido no banco de dados   
        Task<Pedido?> ObterPorIdAsync(Guid id); // Processamento em memória, não persistido no banco de dados   
    }
}