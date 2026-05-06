
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{

    public interface IClienteRepository
    {
        Task<Cliente> ObterPorIdAsync(Guid id);
    }
}

