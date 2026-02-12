using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Commands.CreateSchedule;

public record CreateScheduleCommand(
    Guid PageId, string Content, DateTime ScheduledFor,
    string? ImageUrl, List<string>? Hashtags, string? CallToAction) : IRequest<Result<ScheduleDto>>;
