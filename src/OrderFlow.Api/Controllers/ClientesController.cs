using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteRepository clienteRepository, ILogger<ClientesController> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            _logger.LogInformation("Clientes obtidos com sucesso.");
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteDto dto)
        {
            var cliente = new Cliente(dto.Nome, dto.Email);
            await _clienteRepository.AdicionarAsync(cliente);
            _logger.LogInformation(
             "Cliente criado com sucesso. ClienteId={ClienteId}, Email={Email}",
            cliente.Id,
             cliente.Email);
            return Ok(cliente);

        }

    }
}