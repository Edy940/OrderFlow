using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(Cliente cliente);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
    }
}