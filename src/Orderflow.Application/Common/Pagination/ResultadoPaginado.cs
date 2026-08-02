using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Common.Pagination
{
    public class ResultadoPaginado<T>
    {
        public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas { get; set; }

        public ResultadoPaginado(IEnumerable<T> itens, int pagina, int tamanhoPagina, int totalItens)
        {
            ArgumentNullException.ThrowIfNull(itens);
            if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
            if (tamanhoPagina < 1) throw new ArgumentOutOfRangeException(nameof(tamanhoPagina));
            if (totalItens < 0) throw new ArgumentOutOfRangeException(nameof(totalItens));

            Itens = itens;
            Pagina = pagina;
            TamanhoPagina = tamanhoPagina;
            TotalItens = totalItens;
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina);
        }
    }
}
