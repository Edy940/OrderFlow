using AutoMapper;
using Moq;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
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
                .ReturnsAsync((Cliente)null);

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
            await Assert.ThrowsAsync<Exception>(() => service.CriarPedidoAsync(dto));
        }
    }

}
