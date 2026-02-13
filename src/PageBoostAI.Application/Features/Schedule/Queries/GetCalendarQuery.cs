using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetCalendarQuery(Guid UserId, string Month) : IRequest<Result<CalendarDto>>;

public class GetCalendarQueryHandler : IRequestHandler<GetCalendarQuery, Result<CalendarDto>>
{
    public async Task<Result<CalendarDto>> Handle(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get calendar logic
        await Task.CompletedTask;
        return Result<CalendarDto>.Failure("Not implemented yet");
    }
}
