using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            // Verifica problemas de binding
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido ao criar pedido: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Logue campos específicos para diagnóstico
            _logger.LogInformation("Recebido CriarPedido DTO - ClienteId: {ClienteId}, ItensCount: {ItensCount}", dto?.ClienteId, dto?.Itens?.Count ?? 0);
            _logger.LogDebug("Itens detalhados: {@Itens}", dto?.Itens);

            try
            {
                await _pedidoService.CriarPedidoAsync(dto);
                _logger.LogInformation("Pedido criado com sucesso. ClienteId={ClienteId}", dto.ClienteId);
                return Ok("Pedido criado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido. ClienteId={ClienteId} ItensCount={ItensCount}", dto?.ClienteId, dto?.Itens?.Count ?? 0);
                return BadRequest($"Erro ao criar pedido: {ex.Message}");
            }
        }
    }
}