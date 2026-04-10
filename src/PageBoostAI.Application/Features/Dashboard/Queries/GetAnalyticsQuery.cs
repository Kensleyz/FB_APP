using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Dashboard.Queries;

public record GetAnalyticsQuery(Guid UserId, Guid PageId, string Period) : IRequest<Result<AnalyticsDto>>;

public class GetAnalyticsQueryHandler : IRequestHandler<GetAnalyticsQuery, Result<AnalyticsDto>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly IEncryptionService _encryptionService;

    public GetAnalyticsQueryHandler(
        IFacebookPageRepository facebookPageRepository,
        IContentScheduleRepository contentScheduleRepository,
        IFacebookGraphService facebookGraphService,
        IEncryptionService encryptionService)
    {
        _facebookPageRepository = facebookPageRepository;
        _contentScheduleRepository = contentScheduleRepository;
        _facebookGraphService = facebookGraphService;
        _encryptionService = encryptionService;
    }

    public async Task<Result<AnalyticsDto>> Handle(GetAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var page = await _facebookPageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<AnalyticsDto>.Failure("Page not found.");

        var days = ParsePeriodDays(request.Period);
        var since = DateTime.UtcNow.AddDays(-days).Date;

        var accessToken = _encryptionService.Decrypt(page.PageAccessToken);
        var insights = await _facebookGraphService.GetInsightsAsync(accessToken, page.FacebookPageId, cancellationToken);

        // Build daily breakdown from published content schedules
        var allSchedules = await _contentScheduleRepository.GetByPageIdAsync(page.Id, cancellationToken);
        var publishedInPeriod = allSchedules
            .Where(s => s.Status == PostStatus.Published && s.PublishedAt >= since)
            .ToList();

        var dailyBreakdown = publishedInPeriod
            .GroupBy(s => s.PublishedAt!.Value.Date)
            .Select(g => new DailyAnalyticsDto(g.Key, insights.Reach / Math.Max(days, 1), (int)insights.EngagementRate, g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        var dto = new AnalyticsDto(
            page.Id,
            page.PageName,
            request.Period,
            TotalReach: insights.Reach,
            TotalEngagement: (int)insights.EngagementRate,
            PostCount: publishedInPeriod.Count,
            DailyBreakdown: dailyBreakdown);

        return Result<AnalyticsDto>.Success(dto);
    }

    private static int ParsePeriodDays(string period) => period.ToLowerInvariant() switch
    {
        "7d"  => 7,
        "30d" => 30,
        "90d" => 90,
        _     => 30
    };
}
