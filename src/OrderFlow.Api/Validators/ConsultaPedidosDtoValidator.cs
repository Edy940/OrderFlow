using FluentValidation;
using OrderFlow.Application.DTO;

namespace OrderFlow.Api.Validators;

public class ConsultaPedidosDtoValidator : AbstractValidator<ConsultaPedidosDto>
{
    private static readonly string[] CamposOrdenacao = ["data", "cliente", "valortotal"];
    private static readonly string[] Direcoes = ["asc", "desc"];

    public ConsultaPedidosDtoValidator()
    {
        RuleFor(x => x.Pagina).GreaterThanOrEqualTo(1);
        RuleFor(x => x.TamanhoPagina).InclusiveBetween(1, 100);
        RuleFor(x => x.ValorMinimo).GreaterThanOrEqualTo(0).When(x => x.ValorMinimo.HasValue);
        RuleFor(x => x.ValorMaximo).GreaterThanOrEqualTo(0).When(x => x.ValorMaximo.HasValue);
        RuleFor(x => x).Must(x => !x.DataInicio.HasValue || !x.DataFim.HasValue || x.DataInicio <= x.DataFim)
            .WithMessage("A data inicial não pode ser posterior à data final.");
        RuleFor(x => x).Must(x => !x.ValorMinimo.HasValue || !x.ValorMaximo.HasValue || x.ValorMinimo <= x.ValorMaximo)
            .WithMessage("O valor mínimo não pode ser maior que o valor máximo.");
        RuleFor(x => x.OrdenarPor).Must(v => CamposOrdenacao.Contains(v, StringComparer.OrdinalIgnoreCase))
            .WithMessage("OrdenarPor deve ser: data, cliente ou valorTotal.");
        RuleFor(x => x.Direcao).Must(v => Direcoes.Contains(v, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Direcao deve ser: asc ou desc.");
    }
}
