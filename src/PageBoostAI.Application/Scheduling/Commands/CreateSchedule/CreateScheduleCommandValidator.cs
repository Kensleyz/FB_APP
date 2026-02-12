using FluentValidation;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Scheduling.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.PageId)
            .NotEmpty().WithMessage("Page ID is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(PostContent.MaxLength)
            .WithMessage($"Content cannot exceed {PostContent.MaxLength} characters.");

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future.");
    }
}
