using Common.DTOs.Ban.Request;
using FluentValidation;

namespace Validators.Ban;

public class BanCreateValidation : AbstractValidator<BanCreateModel>
{
    public BanCreateValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
        
        RuleFor(x => x.Reason)
            .MaximumLength(100)
            .WithMessage("Reason is too long");
        
        RuleFor(x => x.BanEndDate)
            .NotEmpty()
            .WithMessage("EndDate is required");
        
        RuleFor(x => x.BanEndDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("EndDate must be in the future");
    }
}