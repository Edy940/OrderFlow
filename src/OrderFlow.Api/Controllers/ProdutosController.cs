using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTO;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            return Ok(produtos);
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoDto dto)
        {
            var produto = new Produto(dto.Nome, dto.Preco, dto.Estoque);

            await _produtoRepository.AdicionarAsync(produto);

            return Ok(produto);
        }
    }
}