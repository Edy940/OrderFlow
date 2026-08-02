
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Common.Pagination;
using Asp.Versioning;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var response = await _pedidoService.ObterPorIdAsync(id);
            if (response == null)
                return NotFound("Pedido não encontrado.");

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] ConsultaPedidosDto parametros)
        {
            var (itens, totalItens) = await _pedidoService.ObterPaginadoAsync(parametros);

            var response = new ResultadoPaginado<PedidoResponseDto>(
                itens,
                parametros.Pagina,
                parametros.TamanhoPagina,
                totalItens);

            return Ok(response);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            if (dto is null)
                return BadRequest("O corpo da requisição é obrigatório.");

            // Verifica problemas de binding
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido ao criar pedido: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Logue campos específicos para diagnóstico
            _logger.LogInformation("Recebido CriarPedido DTO - ClienteId: {ClienteId}, ItensCount: {ItensCount}", dto.ClienteId, dto.Itens?.Count ?? 0);
            _logger.LogDebug("Itens detalhados: {@Itens}", dto.Itens);

            await _pedidoService.CriarPedidoAsync(dto);
            _logger.LogInformation("Pedido criado com sucesso. ClienteId={ClienteId}", dto.ClienteId);
            return Ok("Pedido criado com sucesso.");
        }
    }
}
