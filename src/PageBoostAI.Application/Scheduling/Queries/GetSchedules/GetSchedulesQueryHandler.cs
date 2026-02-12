using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Scheduling.Queries.GetSchedules;

public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, Result<List<ScheduleDto>>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSchedulesQueryHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<ScheduleDto>>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<List<ScheduleDto>>.Failure("User is not authenticated.");

        var userPages = await _pageRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        var userPageIds = userPages.Select(p => p.Id).ToHashSet();

        IReadOnlyList<ContentSchedule> schedules;

        if (request.PageId.HasValue)
        {
            if (!userPageIds.Contains(request.PageId.Value))
                return Result<List<ScheduleDto>>.Failure("Page not found or access denied.");

            schedules = await _scheduleRepository.GetByPageIdAsync(request.PageId.Value, cancellationToken);
        }
        else
        {
            var allSchedules = new List<ContentSchedule>();
            foreach (var pageId in userPageIds)
            {
                var pageSchedules = await _scheduleRepository.GetByPageIdAsync(pageId, cancellationToken);
                allSchedules.AddRange(pageSchedules);
            }
            schedules = allSchedules;
        }

        var dtos = schedules.Select(MapToDto).ToList();
        return Result<List<ScheduleDto>>.Success(dtos);
    }

    private static ScheduleDto MapToDto(ContentSchedule s) => new(
        s.Id, s.PageId, s.Content.Text, s.ImageUrl, s.Hashtags,
        s.CallToAction, s.ScheduledFor, s.Status.ToString(), s.FacebookPostId,
        s.ErrorMessage, s.RetryCount, s.PublishedAt, s.CreatedAt);
}
