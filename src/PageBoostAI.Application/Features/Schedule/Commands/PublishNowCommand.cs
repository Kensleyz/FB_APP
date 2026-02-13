using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record PublishNowCommand(Guid UserId, Guid ScheduleId) : IRequest<Result<ScheduleDto>>;

public class PublishNowCommandHandler : IRequestHandler<PublishNowCommand, Result<ScheduleDto>>
{
    public async Task<Result<ScheduleDto>> Handle(PublishNowCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement publish now logic
        await Task.CompletedTask;
        return Result<ScheduleDto>.Failure("Not implemented yet");
    }
}
