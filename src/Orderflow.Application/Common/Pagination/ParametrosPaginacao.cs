using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Common.Pagination
{
    public class ParametrosPaginacao
    {
        private const int MaximoItensPorPagina = 100;
        public int Pagina { get; set; } = 1;
        private int _tamanhoPagina = 10;

        public int TamanhoPagina
        {
            get => _tamanhoPagina;
            set => _tamanhoPagina = Math.Min(value, MaximoItensPorPagina);
        }
    }
}
