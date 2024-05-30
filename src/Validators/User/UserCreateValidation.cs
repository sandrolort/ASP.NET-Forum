using Common.DTOs.User.Request;
using FluentValidation;

namespace Validators.User;

public class UserCreateValidation : AbstractValidator<UserCreateModel>
{
    public UserCreateValidation()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("Username is required");
        
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(u => u.Email)
            .EmailAddress() // By default, this is Asp.Net compatible.
            .WithMessage("Invalid Email");

        RuleFor(u => u.Email)
            .MaximumLength(80)
            .WithMessage("Email length too great! Maximum of 80 characters allowed.");
        
        RuleFor(u => u.ProfilePicUrl)
            .MaximumLength(100)
            .WithMessage("Profile picture URL must be at most 100 characters long");
        
        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        
        RuleFor(u => u.Password)
            .MinimumLength(10)
            .WithMessage("Password must be at least 10 characters long");
        
        RuleFor(u => u.Password)
            .Matches(@".*\d+.*")
            .WithMessage("Password must contain at least one digit");
    }
}