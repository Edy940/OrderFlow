using OrderFlow.Application.Common.Pagination;

namespace OrderFlow.Application.DTO;

public class ConsultaPedidosDto : ParametrosPaginacao
{
    public Guid? ClienteId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public string OrdenarPor { get; set; } = "data";
    public string Direcao { get; set; } = "desc";
}
