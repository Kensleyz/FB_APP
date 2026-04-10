using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record ConnectFacebookCommand(Guid UserId, string RedirectUri) : IRequest<Result<string>>;

public class ConnectFacebookCommandHandler : IRequestHandler<ConnectFacebookCommand, Result<string>>
{
    private readonly IFacebookGraphService _facebookGraphService;

    public ConnectFacebookCommandHandler(IFacebookGraphService facebookGraphService)
    {
        _facebookGraphService = facebookGraphService;
    }

    public Task<Result<string>> Handle(ConnectFacebookCommand request, CancellationToken cancellationToken)
    {
        // Encode userId and frontend redirect URI in state so we can retrieve them in the callback
        var state = $"{request.UserId}|{request.RedirectUri}";
        var authUrl = _facebookGraphService.BuildAuthUrl(state);
        return Task.FromResult(Result<string>.Success(authUrl));
    }
}
