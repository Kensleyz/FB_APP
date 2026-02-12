using FluentValidation;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Scheduling.Commands.UpdateSchedule;

public class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty().WithMessage("Schedule ID is required.");

        RuleFor(x => x.Content)
            .MaximumLength(PostContent.MaxLength)
            .WithMessage($"Content cannot exceed {PostContent.MaxLength} characters.")
            .When(x => x.Content is not null);

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future.")
            .When(x => x.ScheduledFor.HasValue);
    }
}
