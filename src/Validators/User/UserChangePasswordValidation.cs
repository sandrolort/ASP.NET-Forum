using Common.DTOs.User.Request;
using FluentValidation;

namespace Validators.User;

public class UserChangePasswordValidation : AbstractValidator<UserChangePasswordModel>
{
    public UserChangePasswordValidation()
    {
        RuleFor(u => u.OldPassword)
            .NotEmpty()
            .WithMessage("Old password is required");
        
        RuleFor(u => u.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required");
        
        RuleFor(u => u.NewPassword)
            .MinimumLength(10)
            .WithMessage("New password must be at least 10 characters long");
        
        RuleFor(u => u.NewPassword)
            .Matches(@".*\d+.*")
            .WithMessage("New password must contain at least one digit");
    }
}