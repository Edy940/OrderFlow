
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using System.Runtime.CompilerServices;

namespace OrderFlow.Application.Services
{
    public class PedidoService : IPedidoService
    {

        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IClienteRepository _clienteRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IProdutoRepository produtoRepository, IClienteRepository clienteRepository)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task CriarPedidoAsync(CriarPedidoDto dto) { 
            var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId);
            if (cliente == null)
                throw new Exception("Cliente não encontrado");
            var pedido = new Pedido(cliente);
         
            foreach (var item in dto.Itens)
            {
                var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (produto == null)
                    throw new Exception($"Produto com ID {item.ProdutoId} não encontrado");
                pedido.AdicionarItem(produto, item.Quantidade);
                
            }
            await _pedidoRepository.AdicionarAsync(pedido);
        }
    }
}
