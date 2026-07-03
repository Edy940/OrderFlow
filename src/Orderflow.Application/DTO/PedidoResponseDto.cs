
namespace OrderFlow.Application.DTO
{
    public class PedidoResponseDto
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; }
        public DateTime Data { get; set; }
        public List<ItemPedidoResponseDto> Itens { get; set; } = new();
        public decimal Total { get; set; }
    }
}
