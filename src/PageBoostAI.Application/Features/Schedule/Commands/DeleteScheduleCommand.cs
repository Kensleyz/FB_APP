using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record DeleteScheduleCommand(Guid UserId, Guid ScheduleId) : IRequest<Result>;

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result>
{
    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;

    public DeleteScheduleCommandHandler(
        IContentScheduleRepository contentScheduleRepository,
        IFacebookPageRepository facebookPageRepository)
    {
        _contentScheduleRepository = contentScheduleRepository;
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _contentScheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result.Failure("Schedule not found.");

        var page = await _facebookPageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result.Failure("Schedule not found.");

        try
        {
            schedule.Cancel();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _contentScheduleRepository.UpdateAsync(schedule, cancellationToken);

        return Result.Success();
    }
}
