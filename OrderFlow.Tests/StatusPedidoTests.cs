using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Tests
{
    public class StatusPedidoTests
    {
        [Fact]
        public void NovoPedido_IniciandoComStatusCriado()
        {
            var cliente = new Cliente("Cliente Teste", "cliente@test.com");
            var pedido = new Pedido(cliente);
            Assert.Equal(StatusPedido.Criado, pedido.Status);
        }
    }
}
