using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Entities
{
    //Encapsulamento
    public class ItemPedido
    {
        public Guid Id { get; private set; }
        public Produto Produto { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public ItemPedido(Produto produto, int quantidade, decimal precoUnitario)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
            if (precoUnitario <= 0)
                throw new ArgumentException("O preço unitário deve ser maior que zero.", nameof(precoUnitario));
            if (produto == null)
                throw new ArgumentNullException(nameof(produto), "O produto não pode ser nulo.");
            Id = Guid.NewGuid();
            Produto = produto;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
        //Entidade
        public decimal CalcularValorTotal()
        {
            return Quantidade * PrecoUnitario;
        }
    }
}
