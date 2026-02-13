using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Facebook.Queries;

public record GetUserPagesQuery(Guid UserId) : IRequest<Result<List<FacebookPageDto>>>;

public class GetUserPagesQueryHandler : IRequestHandler<GetUserPagesQuery, Result<List<FacebookPageDto>>>
{
    public async Task<Result<List<FacebookPageDto>>> Handle(GetUserPagesQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get user pages logic
        await Task.CompletedTask;
        return Result<List<FacebookPageDto>>.Failure("Not implemented yet");
    }
}
