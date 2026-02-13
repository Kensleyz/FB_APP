using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record CreateScheduleCommand(
    Guid UserId,
    Guid PageId,
    string Content,
    DateTime ScheduledFor,
    string? ImageUrl,
    string? Hashtags,
    string? CallToAction
) : IRequest<Result<ScheduleDto>>;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<ScheduleDto>>
{
    public async Task<Result<ScheduleDto>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement create schedule logic
        await Task.CompletedTask;
        return Result<ScheduleDto>.Failure("Not implemented yet");
    }
}
