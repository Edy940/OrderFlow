using Microsoft.AspNetCore.Mvc;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteDto dto)
        {
            var cliente = new Cliente(dto.Nome, dto.Email);
            await _clienteRepository.AdicionarAsync(cliente);
            return Ok(cliente);

        }

    }
}