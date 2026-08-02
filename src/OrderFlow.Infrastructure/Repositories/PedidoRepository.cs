using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Domain.Queries;

namespace OrderFlow.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly OrderFlowDbContext _context;

        public PedidoRepository(OrderFlowDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Pedido> Itens, int TotalItens)> ObterPaginadoAsync(PedidoConsulta consulta)
        {
            var query = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .AsQueryable();

            if (consulta.ClienteId.HasValue)
                query = query.Where(p => EF.Property<Guid>(p, "ClienteId") == consulta.ClienteId.Value);

            if (consulta.DataInicio.HasValue)
                query = query.Where(p => p.Data >= consulta.DataInicio.Value);

            if (consulta.DataFim.HasValue)
                query = query.Where(p => p.Data <= consulta.DataFim.Value);

            if (consulta.ValorMinimo.HasValue)
                query = query.Where(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario) >= consulta.ValorMinimo.Value);

            if (consulta.ValorMaximo.HasValue)
                query = query.Where(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario) <= consulta.ValorMaximo.Value);

            var totalItens = await query.CountAsync();

            var itens = await Ordenar(query, consulta)
                .Skip((consulta.Pagina - 1) * consulta.TamanhoPagina)
                .Take(consulta.TamanhoPagina)
                .ToListAsync();

            return (itens, totalItens);


        }

        private static IOrderedQueryable<Pedido> Ordenar(IQueryable<Pedido> query, PedidoConsulta consulta)
        {
            var descendente = consulta.Direcao == DirecaoOrdenacao.Descendente;

            return consulta.OrdenarPor switch
            {
                CampoOrdenacaoPedido.Cliente => descendente
                    ? query.OrderByDescending(p => p.Cliente.Nome).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.Cliente.Nome).ThenBy(p => p.Id),
                CampoOrdenacaoPedido.ValorTotal => descendente
                    ? query.OrderByDescending(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.Itens.Sum(i => i.Quantidade * i.PrecoUnitario)).ThenBy(p => p.Id),
                _ => descendente
                    ? query.OrderByDescending(p => p.Data).ThenBy(p => p.Id)
                    : query.OrderBy(p => p.Data).ThenBy(p => p.Id)
            };
        }

        public async Task<Pedido?> ObterPorIdAsync(Guid id)
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                 .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
