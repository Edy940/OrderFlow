using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OrderFlow.Api.Middleware;
using OrderFlow.Api.Validators;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Controllers;

namespace OrderFlow.Tests;

public class Fase2ApiTests
{
    [Fact]
    public void PedidosController_DeveUsarRotaVersionadaV1()
    {
        var rota = Assert.Single(typeof(PedidosController)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
            .Cast<RouteAttribute>());

        Assert.Equal("api/v{version:apiVersion}/[controller]", rota.Template);
    }

    [Fact]
    public void ConsultaPedidosValidator_DeveRejeitarParametrosInvalidos()
    {
        var dto = new ConsultaPedidosDto
        {
            Pagina = 0,
            TamanhoPagina = 101,
            DataInicio = new DateTime(2026, 2, 1),
            DataFim = new DateTime(2026, 1, 1),
            OrdenarPor = "campo-invalido",
            Direcao = "lado"
        };

        var resultado = new ConsultaPedidosDtoValidator().Validate(dto);

        Assert.False(resultado.IsValid);
        Assert.True(resultado.Errors.Count >= 5);
    }

    [Fact]
    public async Task PedidoService_DeveMapearFiltrosParaContratoDoDominio()
    {
        var pedidos = new Mock<IPedidoRepository>();
        PedidoConsulta? consultaRecebida = null;
        pedidos.Setup(r => r.ObterPaginadoAsync(It.IsAny<PedidoConsulta>()))
            .Callback<PedidoConsulta>(c => consultaRecebida = c)
            .ReturnsAsync((Array.Empty<Pedido>(), 0));

        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<IEnumerable<PedidoResponseDto>>(It.IsAny<object>()))
            .Returns(Array.Empty<PedidoResponseDto>());

        var service = new PedidoService(
            Mock.Of<IClienteRepository>(),
            Mock.Of<IProdutoRepository>(),
            pedidos.Object,
            mapper.Object);

        await service.ObterPaginadoAsync(new ConsultaPedidosDto
        {
            Pagina = 2,
            TamanhoPagina = 20,
            ClienteId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ValorMinimo = 100,
            OrdenarPor = "valorTotal",
            Direcao = "asc"
        });

        Assert.NotNull(consultaRecebida);
        Assert.Equal(2, consultaRecebida.Pagina);
        Assert.Equal(20, consultaRecebida.TamanhoPagina);
        Assert.Equal(CampoOrdenacaoPedido.ValorTotal, consultaRecebida.OrdenarPor);
        Assert.Equal(DirecaoOrdenacao.Ascendente, consultaRecebida.Direcao);
        Assert.Equal(100, consultaRecebida.ValorMinimo);
    }

    [Fact]
    public async Task ExceptionMiddleware_DeveRetornarProblemDetails404()
    {
        var ambiente = new Mock<IHostEnvironment>();
        ambiente.SetupGet(a => a.EnvironmentName).Returns(Environments.Development);
        var middleware = new ExceptionMiddleware(
            _ => throw new RecursoNaoEncontradoException("Pedido não encontrado."),
            Mock.Of<ILogger<ExceptionMiddleware>>(),
            ambiente.Object);
        var contexto = new DefaultHttpContext();
        contexto.Request.Path = "/api/v1/pedidos/id";
        contexto.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(contexto);

        contexto.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(contexto.Response.Body);
        Assert.Equal(StatusCodes.Status404NotFound, contexto.Response.StatusCode);
        Assert.Equal("application/problem+json", contexto.Response.ContentType);
        Assert.Equal("Recurso não encontrado", json.RootElement.GetProperty("title").GetString());
        Assert.True(json.RootElement.TryGetProperty("traceId", out _));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void PedidoConsulta_DeveProtegerInvariantesDePaginacao(int pagina, int tamanho)
    {
        Assert.ThrowsAny<ArgumentException>(() => new PedidoConsulta(pagina, tamanho));
    }
}
