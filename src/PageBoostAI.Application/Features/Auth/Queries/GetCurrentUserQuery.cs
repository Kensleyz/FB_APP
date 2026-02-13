using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Auth.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get current user logic
        await Task.CompletedTask;
        return Result<UserProfileDto>.Failure("Not implemented yet");
    }
}
