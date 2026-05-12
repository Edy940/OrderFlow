using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;


namespace OrderFlow.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private static readonly List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente("João Silva", "joao@email.com")
        };

        public Task<Cliente> ObterPorIdAsync(Guid id)
        {
           
            return Task.FromResult(_clientes.FirstOrDefault(c => c.Id == id));
        }

        public Task AdicionarAsync(Cliente cliente)
        {
            _clientes.Add(cliente);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Cliente>>(_clientes);
        }
    }
}
