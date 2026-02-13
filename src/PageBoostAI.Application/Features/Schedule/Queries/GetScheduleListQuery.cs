using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetScheduleListQuery(Guid UserId, Guid? PageId) : IRequest<Result<List<ScheduleDto>>>;

public class GetScheduleListQueryHandler : IRequestHandler<GetScheduleListQuery, Result<List<ScheduleDto>>>
{
    public async Task<Result<List<ScheduleDto>>> Handle(GetScheduleListQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get schedule list logic
        await Task.CompletedTask;
        return Result<List<ScheduleDto>>.Failure("Not implemented yet");
    }
}
