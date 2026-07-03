using OrderFlow.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Application.Interfaces
{
    public interface IPedidoService
    {
        Task CriarPedidoAsync(CriarPedidoDto dto);
        Task<PedidoResponseDto?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync();
    }
}
