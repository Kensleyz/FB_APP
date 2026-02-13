using MediatR;
using PageBoostAI.Application.Common;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record ConnectFacebookCommand(Guid UserId, string RedirectUri) : IRequest<Result<string>>;

public class ConnectFacebookCommandHandler : IRequestHandler<ConnectFacebookCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConnectFacebookCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement Facebook connect logic
        await Task.CompletedTask;
        return Result<string>.Failure("Not implemented yet");
    }
}
