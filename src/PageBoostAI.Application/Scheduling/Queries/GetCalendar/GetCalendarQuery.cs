using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Scheduling.DTOs;

namespace PageBoostAI.Application.Scheduling.Queries.GetCalendar;

public record GetCalendarQuery(string Month) : IRequest<Result<CalendarDto>>;
