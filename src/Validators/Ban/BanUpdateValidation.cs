using Common.DTOs.Ban.Request;
using FluentValidation;

namespace Validators.Ban;

public class BanUpdateValidation : AbstractValidator<BanUpdateModel>
{
    public BanUpdateValidation()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(100)
            .WithMessage("Reason is too long");
        
        RuleFor(x => x.BanEndDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("EndDate must be in the future");
    }
}