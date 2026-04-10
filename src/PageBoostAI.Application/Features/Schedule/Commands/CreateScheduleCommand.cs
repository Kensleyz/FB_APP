using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Features.Schedule.Commands;

public record CreateScheduleCommand(
    Guid UserId,
    Guid PageId,
    string Content,
    DateTime ScheduledFor,
    string? ImageUrl,
    string? Hashtags,
    string? CallToAction
) : IRequest<Result<ScheduleDto>>;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<ScheduleDto>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IContentScheduleRepository _contentScheduleRepository;

    public CreateScheduleCommandHandler(
        IFacebookPageRepository facebookPageRepository,
        IContentScheduleRepository contentScheduleRepository)
    {
        _facebookPageRepository = facebookPageRepository;
        _contentScheduleRepository = contentScheduleRepository;
    }

    public async Task<Result<ScheduleDto>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var page = await _facebookPageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<ScheduleDto>.Failure("Page not found.");

        var postsToday = await _contentScheduleRepository.CountByPageIdAndDateAsync(request.PageId, request.ScheduledFor, cancellationToken);
        if (!ContentSchedule.CanScheduleMore(postsToday))
            return Result<ScheduleDto>.Failure("Maximum of 4 posts per day per page has been reached.");

        PostContent content;
        try
        {
            content = new PostContent(request.Content);
        }
        catch (ArgumentException ex)
        {
            return Result<ScheduleDto>.Failure(ex.Message);
        }

        var hashtags = request.Hashtags?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        ContentSchedule schedule;
        try
        {
            schedule = new ContentSchedule(request.PageId, content, request.ScheduledFor, request.ImageUrl, hashtags, request.CallToAction);
        }
        catch (DomainException ex)
        {
            return Result<ScheduleDto>.Failure(ex.Message);
        }

        await _contentScheduleRepository.AddAsync(schedule, cancellationToken);

        return Result<ScheduleDto>.Success(ToDto(schedule, page.PageName));
    }

    private static ScheduleDto ToDto(ContentSchedule s, string pageName) =>
        new(s.Id, s.PageId, pageName, s.Content.Text, s.Status.ToString(),
            s.ScheduledFor, s.PublishedAt, s.ImageUrl, s.Hashtags, s.CallToAction, s.FacebookPostId, s.CreatedAt);
}
