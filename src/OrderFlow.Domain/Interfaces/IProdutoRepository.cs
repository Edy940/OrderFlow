using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(Produto produto);
        Task<IEnumerable<Produto>> ObterTodosAsync();
    }
}
