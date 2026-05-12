using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;


namespace OrderFlow.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private static readonly List<Produto> produtos = new()
        {
            new Produto("Notebook", 3500m, 10)
        };

        public Task<Produto> ObterPorIdAsync(Guid id)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(produto);
        }

        public Task AdicionarAsync(Produto produto)
        {
            produtos.Add(produto);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Produto>> ObterTodosAsync()
        {
            return Task.FromResult<IEnumerable<Produto>>(produtos.ToList());
        }
    }
}
