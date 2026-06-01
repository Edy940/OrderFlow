using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Application.DTO;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;
        private readonly IPedidoRepository _pedidoRepository;

        public PedidosController(
             IPedidoService pedidoService,
             IPedidoRepository pedidoRepository,
             ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _pedidoRepository = pedidoRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            var response = new PedidoResponseDto
            {
                Id = pedido.Id,
                Cliente = pedido.Cliente.Nome,
                Data = pedido.Data,
                Total = pedido.Itens.Sum(i => i.Quantidade * i.Produto.Preco),
                Itens = pedido.Itens.Select(i => new ItemPedidoResponseDto
                {
                    Produto = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.Produto.Preco,
                    Subtotal = i.Quantidade * i.Produto.Preco
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            var response = pedidos.Select(p => new PedidoResponseDto
            {
                Id = p.Id,
                Cliente = p.Cliente.Nome,
                Data = p.Data,
                Total = p.Itens.Sum(i => i.Quantidade * i.Produto.Preco),
                Itens = p.Itens.Select(i => new ItemPedidoResponseDto
                {
                    Produto = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.Produto.Preco,
                    Subtotal = i.Quantidade * i.Produto.Preco
                }).ToList()
            }).ToList();

            return Ok(response);
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