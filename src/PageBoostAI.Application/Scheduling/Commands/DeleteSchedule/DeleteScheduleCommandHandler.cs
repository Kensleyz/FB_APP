using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Scheduling.Commands.DeleteSchedule;

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result<bool>>
{
    private readonly IContentScheduleRepository _scheduleRepository;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteScheduleCommandHandler(
        IContentScheduleRepository scheduleRepository,
        IFacebookPageRepository pageRepository,
        ICurrentUserService currentUserService)
    {
        _scheduleRepository = scheduleRepository;
        _pageRepository = pageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<bool>.Failure("User is not authenticated.");

        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<bool>.Failure("Schedule not found.");

        var page = await _pageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != userId.Value)
            return Result<bool>.Failure("Schedule not found or access denied.");

        schedule.Cancel();
        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

        return Result<bool>.Success(true);
    }
}
