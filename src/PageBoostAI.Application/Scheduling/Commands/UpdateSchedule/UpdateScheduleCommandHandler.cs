using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Scheduling.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateScheduleCommandHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ScheduleDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
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

        var content = request.Content is not null ? new PostContent(request.Content) : null;

        schedule.Update(content, request.ScheduledFor, request.ImageUrl, request.Hashtags, request.CallToAction);
        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

        return Result<ScheduleDto>.Success(MapToDto(schedule));
    }

    private static ScheduleDto MapToDto(ContentSchedule s) => new(
        s.Id, s.PageId, s.Content.Text, s.ImageUrl, s.Hashtags,
        s.CallToAction, s.ScheduledFor, s.Status.ToString(), s.FacebookPostId,
        s.ErrorMessage, s.RetryCount, s.PublishedAt, s.CreatedAt);
}
