using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Scheduling.Commands.CreateSchedule;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateScheduleCommandHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ScheduleDto>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<ScheduleDto>.Failure("User is not authenticated.");

        var page = await _pageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != userId.Value)
            return Result<ScheduleDto>.Failure("Page not found or access denied.");

        if (!ContentSchedule.CanSchedule(request.ScheduledFor))
            return Result<ScheduleDto>.Failure("Cannot schedule posts between 10 PM and 6 AM.");

        var postsToday = await _scheduleRepository.CountByPageIdAndDateAsync(
            request.PageId, request.ScheduledFor.Date, cancellationToken);
        if (!ContentSchedule.CanScheduleMore(postsToday))
            return Result<ScheduleDto>.Failure("Maximum of 4 posts per day has been reached.");

        var content = new PostContent(request.Content);
        var schedule = new ContentSchedule(
            request.PageId, content, request.ScheduledFor,
            request.ImageUrl, request.Hashtags, request.CallToAction);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);

        return Result<ScheduleDto>.Success(MapToDto(schedule));
    }

    private static ScheduleDto MapToDto(ContentSchedule s) => new(
        s.Id, s.PageId, s.Content.Text, s.ImageUrl, s.Hashtags,
        s.CallToAction, s.ScheduledFor, s.Status.ToString(), s.FacebookPostId,
        s.ErrorMessage, s.RetryCount, s.PublishedAt, s.CreatedAt);
}
