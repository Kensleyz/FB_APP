using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record UpdateScheduleCommand(
    Guid UserId,
    Guid ScheduleId,
    string Content,
    DateTime ScheduledFor,
    string? ImageUrl,
    string? Hashtags,
    string? CallToAction
) : IRequest<Result<ScheduleDto>>;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result<ScheduleDto>>
{
    private readonly IContentScheduleRepository _contentScheduleRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;

    public UpdateScheduleCommandHandler(
        IContentScheduleRepository contentScheduleRepository,
        IFacebookPageRepository facebookPageRepository)
    {
        _contentScheduleRepository = contentScheduleRepository;
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result<ScheduleDto>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _contentScheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        var page = await _facebookPageRepository.GetByIdAsync(schedule.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<ScheduleDto>.Failure("Schedule not found.");

        PostContent? content = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(request.Content))
                content = new PostContent(request.Content);
        }
        catch (ArgumentException ex)
        {
            return Result<ScheduleDto>.Failure(ex.Message);
        }

        var hashtags = request.Hashtags?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        try
        {
            schedule.Update(content, request.ScheduledFor, request.ImageUrl, hashtags, request.CallToAction);
        }
        catch (DomainException ex)
        {
            return Result<ScheduleDto>.Failure(ex.Message);
        }

        await _contentScheduleRepository.UpdateAsync(schedule, cancellationToken);

        return Result<ScheduleDto>.Success(
            new ScheduleDto(schedule.Id, schedule.PageId, page.PageName, schedule.Content.Text,
                schedule.Status.ToString(), schedule.ScheduledFor, schedule.PublishedAt,
                schedule.ImageUrl, schedule.Hashtags, schedule.CallToAction, schedule.FacebookPostId, schedule.CreatedAt));
    }
}
