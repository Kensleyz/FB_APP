using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record UpdateScheduleCommand(
    Guid UserId,
    Guid ScheduleId,
    string Content,
    DateTime ScheduledFor,
    string? ImageUrl,
    string? Hashtags,
    string? CallToAction
) : IRequest<Result<ScheduleDto>>;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<ScheduleDto>>
{
    public async Task<Result<ScheduleDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement update schedule logic
        await Task.CompletedTask;
        return Result<ScheduleDto>.Failure("Not implemented yet");
    }
}
