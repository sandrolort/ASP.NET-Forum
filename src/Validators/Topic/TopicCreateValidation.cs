using Common.DTOs.Topic.Request;
using FluentValidation;

namespace Validators.Topic;

public class TopicCreateValidation : AbstractValidator<TopicCreateModel>
{
    public TopicCreateValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(80)
            .WithMessage("Name is required and must be less than 80 characters");
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000)
            .WithMessage("Content is required and must be less than 2000 characters");
        
        RuleFor(x => x.BackgroundImageUrl)
            .MaximumLength(300)
            .WithMessage("Background image url must be less than 300 characters");
    }
}