using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Schedule.Queries;

public record GetScheduleByIdQuery(Guid UserId, Guid ScheduleId) : IRequest<Result<ScheduleDto>>;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, Result<ScheduleDto>>
{
    public async Task<Result<ScheduleDto>> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get schedule by id logic
        await Task.CompletedTask;
        return Result<ScheduleDto>.Failure("Not implemented yet");
    }
}
