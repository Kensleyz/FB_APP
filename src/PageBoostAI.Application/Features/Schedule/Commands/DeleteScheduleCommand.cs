using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record DeleteScheduleCommand(Guid UserId, Guid ScheduleId) : IRequest<Result>;

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result>
{
    public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement delete schedule logic
        await Task.CompletedTask;
        return Result.Failure("Not implemented yet");
    }
}
