using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    Guid ScheduleId, string? Content, DateTime? ScheduledFor,
    string? ImageUrl, List<string>? Hashtags, string? CallToAction) : IRequest<Result<ScheduleDto>>;
