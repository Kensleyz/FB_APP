using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetScheduleListQuery(Guid UserId, Guid? PageId) : IRequest<Result<List<ScheduleDto>>>;

public class GetScheduleListQueryHandler : IRequestHandler<GetScheduleListQuery, Result<List<ScheduleDto>>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IContentScheduleRepository _contentScheduleRepository;

    public GetScheduleListQueryHandler(
        IFacebookPageRepository facebookPageRepository,
        IContentScheduleRepository contentScheduleRepository)
    {
        _facebookPageRepository = facebookPageRepository;
        _contentScheduleRepository = contentScheduleRepository;
    }

    public async Task<Result<List<ScheduleDto>>> Handle(GetScheduleListQuery request, CancellationToken cancellationToken)
    {
        var userPages = await _facebookPageRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var pageMap = userPages.ToDictionary(p => p.Id, p => p.PageName);

        // If filtering by page, verify ownership
        if (request.PageId.HasValue && !pageMap.ContainsKey(request.PageId.Value))
            return Result<List<ScheduleDto>>.Failure("Page not found.");

        var pagesToQuery = request.PageId.HasValue
            ? userPages.Where(p => p.Id == request.PageId.Value).ToList()
            : userPages.ToList();

        var allSchedules = new List<ScheduleDto>();
        foreach (var page in pagesToQuery)
        {
            var schedules = await _contentScheduleRepository.GetByPageIdAsync(page.Id, cancellationToken);
            allSchedules.AddRange(schedules.Select(s =>
                new ScheduleDto(s.Id, s.PageId, page.PageName, s.Content.Text, s.Status.ToString(),
                    s.ScheduledFor, s.PublishedAt, s.ImageUrl, s.Hashtags, s.CallToAction, s.FacebookPostId, s.CreatedAt)));
        }

        return Result<List<ScheduleDto>>.Success(allSchedules.OrderByDescending(s => s.ScheduledFor).ToList());
    }
}
