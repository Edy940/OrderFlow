using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task AdicionarAsync(Pedido pedido);
         //Diferente de QueryAble, pois não é uma consulta, mas sim um processamento em memória, não persistido no banco de dados   
        Task<(IEnumerable<Pedido> Itens, int TotalItens)> ObterPaginadoAsync(
            OrderFlow.Domain.Queries.PedidoConsulta consulta);
        Task<Pedido?> ObterPorIdAsync(Guid id); // Processamento em memória, não persistido no banco de dados   
    }
}
