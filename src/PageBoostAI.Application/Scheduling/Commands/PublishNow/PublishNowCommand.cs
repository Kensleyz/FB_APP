using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Commands.PublishNow;

public record PublishNowCommand(Guid ScheduleId) : IRequest<Result<ScheduleDto>>;
