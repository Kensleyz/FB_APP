using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Dashboard.Queries;

public record GetDashboardOverviewQuery(Guid UserId) : IRequest<Result<DashboardOverviewDto>>;

public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
{
    public async Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement dashboard overview logic
        await Task.CompletedTask;
        return Result<DashboardOverviewDto>.Failure("Not implemented yet");
    }
}
