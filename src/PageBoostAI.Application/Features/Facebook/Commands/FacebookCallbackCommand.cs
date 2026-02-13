using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record FacebookCallbackCommand(Guid UserId, string Code, string? State) : IRequest<Result<List<FacebookPageDto>>>;

public class FacebookCallbackCommandHandler : IRequestHandler<FacebookCallbackCommand, Result<List<FacebookPageDto>>>
{
    public async Task<Result<List<FacebookPageDto>>> Handle(FacebookCallbackCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement Facebook callback logic
        await Task.CompletedTask;
        return Result<List<FacebookPageDto>>.Failure("Not implemented yet");
    }
}
