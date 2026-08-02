using OrderFlow.Application.DTO;

namespace OrderFlow.Application.Interfaces;

public interface IClienteService
{
    Task CriarClienteAsync(CriarClienteDto dto);

    Task<ClienteResponseDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<ClienteResponseDto>> ObterTodosAsync();
}