using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Queries.GetSchedules;

public record GetSchedulesQuery(Guid? PageId = null) : IRequest<Result<List<ScheduleDto>>>;
