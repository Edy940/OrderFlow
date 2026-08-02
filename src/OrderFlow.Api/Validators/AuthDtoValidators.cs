using FluentValidation;
using OrderFlow.Application.DTO;

namespace OrderFlow.Api.Validators;

public class RegistrarUsuarioDtoValidator : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("A senha deve conter uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter um número.");
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Senha).NotEmpty();
    }
}

public class RenovarTokenDtoValidator : AbstractValidator<RenovarTokenDto>
{
    public RenovarTokenDtoValidator() => RuleFor(x => x.RefreshToken).NotEmpty();
}

public class RevogarTokenDtoValidator : AbstractValidator<RevogarTokenDto>
{
    public RevogarTokenDtoValidator() => RuleFor(x => x.RefreshToken).NotEmpty();
}
