using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Facebook.Queries;

public record GetPageInsightsQuery(Guid UserId, Guid PageId) : IRequest<Result<PageInsightsDto>>;

public class GetPageInsightsQueryHandler : IRequestHandler<GetPageInsightsQuery, Result<PageInsightsDto>>
{
    public async Task<Result<PageInsightsDto>> Handle(GetPageInsightsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get page insights logic
        await Task.CompletedTask;
        return Result<PageInsightsDto>.Failure("Not implemented yet");
    }
}
