using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Scheduling.Queries.GetCalendar;

public class GetCalendarQueryHandler : IRequestHandler<GetCalendarQuery, Result<CalendarDto>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCalendarQueryHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CalendarDto>> Handle(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<CalendarDto>.Failure("User is not authenticated.");

        if (!DateTime.TryParseExact(request.Month, "yyyy-MM",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var monthDate))
            return Result<CalendarDto>.Failure("Invalid month format. Expected YYYY-MM.");

        var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1);

        var userPages = await _pageRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        var userPageIds = userPages.Select(p => p.Id).ToHashSet();

        var allSchedules = new List<ContentSchedule>();
        foreach (var pageId in userPageIds)
        {
            var pageSchedules = await _scheduleRepository.GetByPageIdAsync(pageId, cancellationToken);
            allSchedules.AddRange(pageSchedules);
        }

        var filtered = allSchedules
            .Where(s => s.ScheduledFor >= startOfMonth && s.ScheduledFor < endOfMonth)
            .OrderBy(s => s.ScheduledFor)
            .Select(MapToDto)
            .ToList();

        return Result<CalendarDto>.Success(new CalendarDto(request.Month, filtered));
    }

    private static ScheduleDto MapToDto(ContentSchedule s) => new(
        s.Id, s.PageId, s.Content.Text, s.ImageUrl, s.Hashtags,
        s.CallToAction, s.ScheduledFor, s.Status.ToString(), s.FacebookPostId,
        s.ErrorMessage, s.RetryCount, s.PublishedAt, s.CreatedAt);
}
