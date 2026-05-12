using Microsoft.AspNetCore.Mvc;
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
    }
}