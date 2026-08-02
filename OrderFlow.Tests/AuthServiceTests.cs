using Moq;
using OrderFlow.Application.DTO;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;

namespace OrderFlow.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegistrarAsync_DeveCriarUsuarioETokens_QuandoEmailForNovo()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        var senhaHasher = new Mock<ISenhaHasher>();
        var tokenService = new Mock<ITokenService>();
        Usuario? usuarioAdicionado = null;

        repositorio.Setup(r => r.ObterPorEmailAsync("novo@email.com")).ReturnsAsync((Usuario?)null);
        repositorio.Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => usuarioAdicionado = u)
            .Returns(Task.CompletedTask);
        senhaHasher.Setup(h => h.GerarHash("Senha123")).Returns("hash-seguro");
        tokenService.Setup(t => t.GerarAccessToken(It.IsAny<Usuario>()))
            .Returns(new AccessTokenGeradoDto("access-token", DateTimeOffset.UtcNow.AddMinutes(15)));
        tokenService.Setup(t => t.GerarRefreshToken()).Returns("refresh-token");
        tokenService.Setup(t => t.CalcularHashToken("refresh-token")).Returns("refresh-hash");
        tokenService.Setup(t => t.ObterExpiracaoRefreshToken()).Returns(DateTimeOffset.UtcNow.AddDays(7));

        var service = new AuthService(repositorio.Object, senhaHasher.Object, tokenService.Object);
        var resultado = await service.RegistrarAsync(new("Novo Usuário", "NOVO@email.com", "Senha123"));

        Assert.True(resultado.Sucesso);
        Assert.Equal("access-token", resultado.Tokens?.AccessToken);
        Assert.Equal("refresh-token", resultado.Tokens?.RefreshToken);
        Assert.Equal("novo@email.com", usuarioAdicionado?.Email);
        Assert.Single(usuarioAdicionado!.RefreshTokens);
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_NaoDeveInformarQualCredencialFalhou()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio.Setup(r => r.ObterPorEmailAsync("inexistente@email.com")).ReturnsAsync((Usuario?)null);
        var service = new AuthService(
            repositorio.Object,
            Mock.Of<ISenhaHasher>(),
            Mock.Of<ITokenService>());

        var resultado = await service.LoginAsync(new("inexistente@email.com", "SenhaInvalida"));

        Assert.False(resultado.Sucesso);
        Assert.Equal("E-mail ou senha inválidos.", resultado.Erro);
    }
}
