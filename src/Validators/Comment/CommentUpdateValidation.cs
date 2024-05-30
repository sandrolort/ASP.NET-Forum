using Common.DTOs.Comment.Request;
using FluentValidation;

namespace Validators.Comment;

public class CommentUpdateValidation : AbstractValidator<CommentUpdateModel>
{
    public CommentUpdateValidation()
    {
        RuleFor(c => c.Content)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Content must not be empty and must be less than 2000 characters");
    }
}