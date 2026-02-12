using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Queries.GetScheduleById;

public record GetScheduleByIdQuery(Guid ScheduleId) : IRequest<Result<ScheduleDto>>;
