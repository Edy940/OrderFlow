namespace OrderFlow.Domain.Queries;

public enum CampoOrdenacaoPedido { Data, Cliente, ValorTotal }
public enum DirecaoOrdenacao { Ascendente, Descendente }

public sealed record PedidoConsulta
{
    public PedidoConsulta(int pagina, int tamanhoPagina, Guid? clienteId = null,
        DateTime? dataInicio = null, DateTime? dataFim = null,
        decimal? valorMinimo = null, decimal? valorMaximo = null,
        CampoOrdenacaoPedido ordenarPor = CampoOrdenacaoPedido.Data,
        DirecaoOrdenacao direcao = DirecaoOrdenacao.Descendente)
    {
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina), "A página deve ser maior ou igual a 1.");
        if (tamanhoPagina is < 1 or > 100) throw new ArgumentOutOfRangeException(nameof(tamanhoPagina), "O tamanho da página deve estar entre 1 e 100.");
        if (dataInicio.HasValue && dataFim.HasValue && dataInicio > dataFim) throw new ArgumentException("A data inicial não pode ser posterior à data final.");
        if (valorMinimo < 0 || valorMaximo < 0) throw new ArgumentException("Os valores de filtro não podem ser negativos.");
        if (valorMinimo.HasValue && valorMaximo.HasValue && valorMinimo > valorMaximo) throw new ArgumentException("O valor mínimo não pode ser maior que o valor máximo.");

        Pagina = pagina; TamanhoPagina = tamanhoPagina; ClienteId = clienteId;
        DataInicio = dataInicio; DataFim = dataFim; ValorMinimo = valorMinimo;
        ValorMaximo = valorMaximo; OrdenarPor = ordenarPor; Direcao = direcao;
    }

    public int Pagina { get; }
    public int TamanhoPagina { get; }
    public Guid? ClienteId { get; }
    public DateTime? DataInicio { get; }
    public DateTime? DataFim { get; }
    public decimal? ValorMinimo { get; }
    public decimal? ValorMaximo { get; }
    public CampoOrdenacaoPedido OrdenarPor { get; }
    public DirecaoOrdenacao Direcao { get; }
}
