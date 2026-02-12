using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Scheduling.Commands.DeleteSchedule;

public record DeleteScheduleCommand(Guid ScheduleId) : IRequest<Result<bool>>;
