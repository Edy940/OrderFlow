
namespace OrderFlow.Application.DTO
{
    public class CriarPedidoDto
    {
        public Guid ClienteId { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }
}
