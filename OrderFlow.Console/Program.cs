
using OrderFlow.Domain.Entities;

var cliente = new Cliente("João Silva", "ben@gmail.com");
var produto = new Produto("Notebook", 3511, 10);
var pedido = new Pedido(cliente);

pedido.AdicionarItem(produto, 5);

/*Console.WriteLine($"Pedido para: {pedido.Cliente.Nome}");
Console.WriteLine($"Email: {pedido.Cliente.Email}");
Console.WriteLine($"Data do Pedido: {pedido.Data}");
Console.WriteLine($"Produto: {produto.Nome}");
Console.WriteLine($"Preço Unitário: {produto.Preco:C}");
Console.WriteLine($"Quantidade:  {produto.Estoque}");
Console.WriteLine($"Valor Total: {pedido.ValorTotal}");*/

Console.WriteLine($"Pedido para: {pedido.Cliente.Nome}");
Console.WriteLine($"Email: {pedido.Cliente.Email}");
Console.WriteLine($"Data do Pedido: {pedido.Data}");
Console.WriteLine($"Produto: {produto.Nome}");
Console.WriteLine($"Preço Unitário: {produto.Preco:C}");

Console.WriteLine(pedido.ValorTotal);


