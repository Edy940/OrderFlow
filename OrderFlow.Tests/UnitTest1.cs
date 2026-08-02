using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Api.Controllers;
using OrderFlow.Application.Common.Pagination;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Infrastructure.Repositories;

namespace OrderFlow.Tests
{
    public class PedidoServiceTests
    {
        [Fact]
        public async Task CriarPedidoAsync_DeveCriarPedido_QuandoDadosForemValidos()
        {
            // Arrange
            var cliente = new Cliente("João Silva", "joao@email.com");
            var produto = new Produto("Notebook", 3500m, 10);

            var clienteRepositoryMock = new Mock<IClienteRepository>();
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var mapperMock = new Mock<IMapper>();

            clienteRepositoryMock
                .Setup(x => x.ObterPorIdAsync(cliente.Id))
                .ReturnsAsync(cliente);

            produtoRepositoryMock
                .Setup(x => x.ObterPorIdAsync(produto.Id))
                .ReturnsAsync(produto);

            var service = new PedidoService(
                clienteRepositoryMock.Object,
                produtoRepositoryMock.Object,
                pedidoRepositoryMock.Object,
                mapperMock.Object // passar o mock aqui
            );

            var dto = new CriarPedidoDto
            {
                ClienteId = cliente.Id,
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        ProdutoId = produto.Id,
                        Quantidade = 2
                    }
                }
            };

            // Act
            await service.CriarPedidoAsync(dto);

            // Assert
            pedidoRepositoryMock.Verify(
                x => x.AdicionarAsync(It.IsAny<Pedido>()),
                Times.Once
            );
        }

    [Fact]
        public async Task CriarPedidoAsync_DeveLancarErro_QuandoClienteNaoExistir()
        {
            // Arrange
            var produto = new Produto("Notebook", 3500m, 10);

            var clienteRepositoryMock = new Mock<IClienteRepository>();
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var mapperMock = new Mock<IMapper>();

            clienteRepositoryMock
                .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Cliente?)null);

            var service = new PedidoService(
                clienteRepositoryMock.Object,
                produtoRepositoryMock.Object,
                pedidoRepositoryMock.Object,
                mapperMock.Object
            );

            var dto = new CriarPedidoDto
            {
                ClienteId = Guid.NewGuid(),
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        ProdutoId = produto.Id,
                        Quantidade = 2
                    }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => service.CriarPedidoAsync(dto));
        }
        [Fact]
        public async Task ObterPaginado_DeveRetornarResultadoPaginado()
        {
            // Arrange
            var serviceMock = new Mock<IPedidoService>();
            var loggerMock = new Mock<ILogger<PedidosController>>();

            var pedidos = new List<PedidoResponseDto>
    {
        new PedidoResponseDto(),
        new PedidoResponseDto()
    };

            var parametros = new ConsultaPedidosDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            serviceMock
                .Setup(service => service.ObterPaginadoAsync(parametros))
                .ReturnsAsync((pedidos, 25));

            var controller = new PedidosController(
                serviceMock.Object,
                loggerMock.Object);

            // Act
            var resultado = await controller.ObterTodos(parametros);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);

            var resposta = Assert.IsType<ResultadoPaginado<PedidoResponseDto>>(
                okResult.Value);

            Assert.Equal(2, resposta.Itens.Count());
            Assert.Equal(25, resposta.TotalItens);
            Assert.Equal(3, resposta.TotalPaginas);
            Assert.Equal(1, resposta.Pagina);
            Assert.Equal(10, resposta.TamanhoPagina);

            serviceMock.Verify(
                service => service.ObterPaginadoAsync(parametros),
                Times.Once);
        }
    }

}
