using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Dashboard.Queries;

public record GetAnalyticsQuery(Guid UserId, Guid PageId, string Period) : IRequest<Result<AnalyticsDto>>;

public class GetAnalyticsQueryHandler : IRequestHandler<GetAnalyticsQuery, Result<AnalyticsDto>>
{
    public async Task<Result<AnalyticsDto>> Handle(GetAnalyticsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement analytics logic
        await Task.CompletedTask;
        return Result<AnalyticsDto>.Failure("Not implemented yet");
    }
}
