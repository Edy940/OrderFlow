using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Entities
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal Estoque { get; set; }

        public Produto(string nome, decimal preco, int estoque)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Preco = preco;
            Estoque = estoque;

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(nome));
            if (preco <= 0)
                throw new ArgumentException("O preço do produto deve ser menor que zero.", nameof(preco));
            if (estoque < 0)
                throw new ArgumentException("O estoque do produto não pode ser negativo.", nameof(estoque));
            if(estoque == 0)
                throw new ArgumentException("Este produto está com estoque zerado.", nameof(estoque));
            if (preco > 10000)
                throw new ArgumentException("Esse produto fornece um desconto de 10%.", nameof(preco));
        }
       
    }       
}
       
