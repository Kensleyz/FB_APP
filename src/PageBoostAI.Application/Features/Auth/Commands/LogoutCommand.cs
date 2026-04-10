using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record LogoutCommand(Guid UserId) : IRequest<Result>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // JWT is stateless — invalidation is handled client-side by discarding the token.
        // If token blacklisting is added later, do it here via ICacheService.
        return Task.FromResult(Result.Success());
    }
}
