using AutoMapper;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public PedidoService(
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
            _mapper = mapper;
        }

        public async Task CriarPedidoAsync(CriarPedidoDto dto)
        {
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

        public async Task<PedidoResponseDto?> ObterPorIdAsync(Guid id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            return pedido == null ? null : _mapper.Map<PedidoResponseDto>(pedido);
        }

        public async Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync()
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<PedidoResponseDto>>(pedidos);
        }
    }
}

