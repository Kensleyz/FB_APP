using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetCalendarQuery(Guid UserId, string Month) : IRequest<Result<CalendarDto>>;

public class GetCalendarQueryHandler : IRequestHandler<GetCalendarQuery, Result<CalendarDto>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IContentScheduleRepository _contentScheduleRepository;

    public GetCalendarQueryHandler(
        IFacebookPageRepository facebookPageRepository,
        IContentScheduleRepository contentScheduleRepository)
    {
        _facebookPageRepository = facebookPageRepository;
        _contentScheduleRepository = contentScheduleRepository;
    }

    public async Task<Result<CalendarDto>> Handle(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParseExact(request.Month, "yyyy-MM",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var monthStart))
        {
            return Result<CalendarDto>.Failure("Invalid month format. Use yyyy-MM.");
        }

        var monthEnd = monthStart.AddMonths(1);

        var userPages = await _facebookPageRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var allScheduleDtos = new List<ScheduleDto>();
        foreach (var page in userPages)
        {
            var schedules = await _contentScheduleRepository.GetByPageIdAsync(page.Id, cancellationToken);
            var inMonth = schedules.Where(s => s.ScheduledFor >= monthStart && s.ScheduledFor < monthEnd);
            allScheduleDtos.AddRange(inMonth.Select(s =>
                new ScheduleDto(s.Id, s.PageId, page.PageName, s.Content.Text, s.Status.ToString(),
                    s.ScheduledFor, s.PublishedAt, s.ImageUrl, s.Hashtags, s.CallToAction, s.FacebookPostId, s.CreatedAt)));
        }

        // Build a day entry for each day that has at least one schedule
        var days = allScheduleDtos
            .GroupBy(s => s.ScheduledFor.Date)
            .Select(g => new CalendarDayDto(g.Key, g.OrderBy(s => s.ScheduledFor).ToList()))
            .OrderBy(d => d.Date)
            .ToList();

        return Result<CalendarDto>.Success(new CalendarDto(request.Month, days));
    }
}
