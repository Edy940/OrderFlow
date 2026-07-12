using AutoMapper;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task CriarClienteAsync(CriarClienteDto dto)
        {
            var cliente = new Cliente(dto.Nome, dto.Email);
            await _clienteRepository.AdicionarAsync(cliente);
        }

        public async Task<ClienteResponseDto?> ObterPorIdAsync(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            return cliente == null ? null : _mapper.Map<ClienteResponseDto>(cliente);
        }

        public async Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<ClienteResponseDto>>(clientes);
        }
    }
}