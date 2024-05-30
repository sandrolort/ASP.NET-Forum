using Common.DTOs.User.Request;
using FluentValidation;

namespace Validators.User;

public class UserLoginValidation : AbstractValidator<UserLoginModel>
{
    public UserLoginValidation()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Username is required");
        
        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        
        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        
        RuleFor(u => u.Password)
            .MinimumLength(10)
            .WithMessage("Password must be at least 10 characters long");
        
        //contains at least one digit
        RuleFor(u => u.Password)
            .Matches(@".*\d+.*")
            .WithMessage("Password must contain at least one digit");
    }
}