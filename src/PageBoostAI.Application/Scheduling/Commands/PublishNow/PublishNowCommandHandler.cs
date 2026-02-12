using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Scheduling.Commands.PublishNow;

public class PublishNowCommandHandler : IRequestHandler<PublishNowCommand, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly ICurrentUserService _currentUserService;

    public PublishNowCommandHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        IFacebookGraphService facebookGraphService,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _facebookGraphService = facebookGraphService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ScheduleDto>> Handle(PublishNowCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<ScheduleDto>.Failure("User is not authenticated.");

        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        var page = await _pageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != userId.Value)
            return Result<ScheduleDto>.Failure("Schedule not found or access denied.");

        try
        {
            var message = schedule.Content.Text;
            if (schedule.Hashtags.Count > 0)
                message += "\n\n" + string.Join(" ", schedule.Hashtags.Select(h => h.StartsWith('#') ? h : $"#{h}"));

            var facebookPostId = await _facebookGraphService.PublishPostAsync(
                page.PageAccessToken, page.FacebookPageId, message, schedule.ImageUrl, cancellationToken);

            schedule.Publish(facebookPostId);
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

            return Result<ScheduleDto>.Success(MapToDto(schedule));
        }
        catch (Exception ex)
        {
            schedule.MarkFailed(ex.Message);
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

            return Result<ScheduleDto>.Failure($"Failed to publish: {ex.Message}");
        }
    }

    private static ScheduleDto MapToDto(ContentSchedule s) => new(
        s.Id, s.PageId, s.Content.Text, s.ImageUrl, s.Hashtags,
        s.CallToAction, s.ScheduledFor, s.Status.ToString(), s.FacebookPostId,
        s.ErrorMessage, s.RetryCount, s.PublishedAt, s.CreatedAt);
}
