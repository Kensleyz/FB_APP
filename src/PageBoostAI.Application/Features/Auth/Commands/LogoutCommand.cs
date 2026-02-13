using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record LogoutCommand(Guid UserId) : IRequest<Result>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement logout logic
        await Task.CompletedTask;
        return Result.Success();
    }
}
