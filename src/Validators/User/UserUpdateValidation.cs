using Common.DTOs.User.Request;
using FluentValidation;

namespace Validators.User;

public class UserUpdateValidation : AbstractValidator<UserUpdateModel>
{
    public UserUpdateValidation()
    {
        RuleFor(u => u.ProfilePicUrl)
            .MaximumLength(100)
            .WithMessage("Profile picture URL must be at most 100 characters long");
    }
}