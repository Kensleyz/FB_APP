using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Dashboard.Queries;

public record GetDashboardOverviewQuery(Guid UserId) : IRequest<Result<DashboardOverviewDto>>;

public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQuery, Result<DashboardOverviewDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IUsageMetricsRepository _usageMetricsRepository;
    private readonly IContentScheduleRepository _contentScheduleRepository;

    public GetDashboardOverviewQueryHandler(
        IUserRepository userRepository,
        IFacebookPageRepository facebookPageRepository,
        IUsageMetricsRepository usageMetricsRepository,
        IContentScheduleRepository contentScheduleRepository)
    {
        _userRepository = userRepository;
        _facebookPageRepository = facebookPageRepository;
        _usageMetricsRepository = usageMetricsRepository;
        _contentScheduleRepository = contentScheduleRepository;
    }

    public async Task<Result<DashboardOverviewDto>> Handle(GetDashboardOverviewQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<DashboardOverviewDto>.Failure("User not found.");

        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");

        var pages = await _facebookPageRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var metrics = await _usageMetricsRepository.GetByUserIdAndPeriodAsync(request.UserId, currentPeriod, cancellationToken);

        var pageIds = pages.Select(p => p.Id).ToHashSet();

        var scheduledPosts = await _contentScheduleRepository.GetByStatusAsync(PostStatus.Scheduled, cancellationToken);
        var scheduledCount = scheduledPosts.Count(s => pageIds.Contains(s.PageId));

        var (maxPosts, maxPages, maxImages) = UsageMetrics.GetLimits(user.SubscriptionTier);

        var usage = new UsageDto(
            currentPeriod,
            metrics?.PostsGenerated ?? 0,
            maxPosts,
            metrics?.ImagesCreated ?? 0,
            maxImages,
            metrics?.PostsPublished ?? 0,
            pages.Count,
            maxPages);

        var dto = new DashboardOverviewDto(
            TotalPages: pages.Count,
            TotalPosts: metrics?.PostsPublished ?? 0,
            PostsThisMonth: metrics?.PostsGenerated ?? 0,
            ScheduledPosts: scheduledCount,
            SubscriptionTier: user.SubscriptionTier.ToString(),
            Usage: usage);

        return Result<DashboardOverviewDto>.Success(dto);
    }
}
