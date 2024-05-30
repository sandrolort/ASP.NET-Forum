using Common.DTOs;
using FluentValidation;

namespace Validators.Token;

public class TokenDtoValidation : AbstractValidator<TokenDto>
{
    public TokenDtoValidation()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .NotNull();
    }
}