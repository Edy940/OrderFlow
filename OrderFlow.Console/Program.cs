
using OrderFlow.Application.DTO;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Repositories;
using System.Text.Json;

var clienteRepository = new ClienteRepository();
var produtoRepository = new ProdutoRepository();
var pedidoRepository = new PedidoRepository();

var pedidoService = new PedidoService(pedidoRepository, produtoRepository, clienteRepository);
;

var cliente = new Cliente("Cliente Gerado", "gerado@example.com");
var produto = new Produto("Produto Gerado", 50m, 5);
// Persiste nas coleções em memória para que PedidoService os encontre
await clienteRepository.AdicionarAsync(cliente);
await produtoRepository.AdicionarAsync(produto);

if (cliente == null)
{
    Console.WriteLine("Nenhum cliente disponível para criar o pedido.");
    return;
}

if (produto == null)
{
    Console.WriteLine("Nenhum produto disponível para criar o pedido.");
    return;
}


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

Console.WriteLine("DTO criado:");
Console.WriteLine(JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));

try
{
    await pedidoService.CriarPedidoAsync(dto);
    Console.WriteLine("Pedido criado com sucesso.");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao criar pedido: {ex.Message}");
}