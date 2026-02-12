using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly ICacheService _cacheService;

    public LogoutCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync($"refresh_token:{request.RefreshToken}", cancellationToken);
        return Result<bool>.Success(true);
    }
}
