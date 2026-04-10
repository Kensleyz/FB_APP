using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetScheduleByIdQuery(Guid UserId, Guid ScheduleId) : IRequest<Result<ScheduleDto>>;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;

    public GetScheduleByIdQueryHandler(
        IContentScheduleRepository contentScheduleRepository,
        IFacebookPageRepository facebookPageRepository)
    {
        _contentScheduleRepository = contentScheduleRepository;
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result<ScheduleDto>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _contentScheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        var page = await _facebookPageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        return Result<ScheduleDto>.Success(
            new ScheduleDto(schedule.Id, schedule.PageId, page.PageName, schedule.Content.Text,
                schedule.Status.ToString(), schedule.ScheduledFor, schedule.PublishedAt,
                schedule.ImageUrl, schedule.Hashtags, schedule.CallToAction, schedule.FacebookPostId, schedule.CreatedAt));
    }
}
