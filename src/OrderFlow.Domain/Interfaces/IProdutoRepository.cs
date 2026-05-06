using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto> ObterPorIdAsync(Guid id);
    }
}
