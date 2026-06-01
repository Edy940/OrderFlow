using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Entities
{
    public class Pedido
    {
        //Entidade que representa um pedido de compra, contendo informações sobre o cliente, data e os itens do pedido
        private readonly List<ItemPedido> _itens = new();
        public Guid Id { get; private set; }
        public Cliente Cliente { get; private set; }
        public DateTime Data { get; private set; }

        // Agregação de itens ao pedido - um pedido pode ter vários itens, mas um item pertence a um único pedido
        public IReadOnlyCollection<ItemPedido> Itens => _itens;
        protected Pedido() { }

        public decimal ValorTotal => _itens.Sum(i => i.CalcularValorTotal());

        public Pedido(Cliente cliente)
        {
            Id = Guid.NewGuid();
            Cliente = cliente;
            Data = DateTime.UtcNow;
        }

        public void AdicionarItem(Produto produto, int quantidade)
        {
            var itemPedido = new ItemPedido(produto, quantidade, produto.Preco);
            _itens.Add(itemPedido);
        }
    }

}




       
