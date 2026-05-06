using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Entities
{
    public class Pedido
    {
        private readonly List<ItemPedido> _itens = new();
        public Guid Id { get; private set; }
        public Cliente Cliente { get; private set; }
        public DateTime Data { get; private set; }

        public IReadOnlyCollection<ItemPedido> Itens => _itens;

        public decimal ValorTotal => _itens.Sum(i => i.CalcularValorTotal());

        public Pedido(Cliente cliente)
        {
            Id = Guid.NewGuid();
            Cliente = cliente;
            Data = DateTime.Now;
        }

        public void AdicionarItem(Produto produto, int quantidade)
        {
            var itemPedido = new ItemPedido(produto, quantidade, produto.Preco);
            _itens.Add(itemPedido);
        }
    }

}




       
