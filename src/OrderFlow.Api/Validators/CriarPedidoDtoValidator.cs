using FluentValidation;
using OrderFlow.Application.DTO;

namespace OrderFlow.Api.Validators
{
    public class CriarPedidoDtoValidator : AbstractValidator<CriarPedidoDto>
    {
        public CriarPedidoDtoValidator()
        {
            RuleFor(x => x.ClienteId).NotEmpty().WithMessage("O ID do cliente é obrigatório.");
            RuleFor(x => x.Itens).NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");
            RuleForEach(x => x.Itens).ChildRules(item =>
            {
                item.RuleFor(i => i.ProdutoId).NotEmpty().WithMessage("O ID do produto é obrigatório.");
                item.RuleFor(i => i.Quantidade).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            });
        }
    }
}
