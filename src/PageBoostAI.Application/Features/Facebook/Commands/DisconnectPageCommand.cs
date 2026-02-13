using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record DisconnectPageCommand(Guid UserId, Guid PageId) : IRequest<Result>;

public class DisconnectPageCommandHandler : IRequestHandler<DisconnectPageCommand, Result>
{
    public async Task<Result> Handle(DisconnectPageCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement disconnect page logic
        await Task.CompletedTask;
        return Result.Failure("Not implemented yet");
    }
}
