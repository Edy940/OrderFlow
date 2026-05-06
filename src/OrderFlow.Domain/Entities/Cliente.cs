using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public Cliente(string nome, string email)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;

            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do cliente não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O email do cliente não pode ser vazio.", nameof(email));
        }
    }

}

