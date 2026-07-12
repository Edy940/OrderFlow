namespace OrderFlow.Application.DTO;

public class PedidoResponseDto
{
    public Guid Id { get; init; }
    public string Cliente { get; init; } = string.Empty;
    public DateTime Data { get; init; }
    public List<ItemPedidoResponseDto> Itens { get; init; } = [];
    public decimal Total { get; init; }
}