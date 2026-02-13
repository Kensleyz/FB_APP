using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Schedule.Commands;
using PageBoostAI.Application.Features.Schedule.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScheduleController(IMediator mediator)
    {
        _mediator = mediator;
    }    [HttpPost]
    public async Task<ActionResult<Result<ScheduleDto>>> Create([FromBody] CreateScheduleDto dto)
    {
        var userId = GetUserId();
        var hashtags = dto.Hashtags != null ? string.Join(",", dto.Hashtags) : null;
        var result = await _mediator.Send(new CreateScheduleCommand(userId, dto.PageId, dto.Content, dto.ScheduledFor, dto.ImageUrl, hashtags, dto.CallToAction));
        if (!result.IsSuccess) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ScheduleDto>>>> GetAll([FromQuery] Guid? pageId = null)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetScheduleListQuery(userId, pageId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<ScheduleDto>>> GetById(Guid id)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetScheduleByIdQuery(userId, id));
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result<ScheduleDto>>> Update(Guid id, [FromBody] UpdateScheduleDto dto)
    {
        var userId = GetUserId();
        var scheduledFor = dto.ScheduledFor ?? DateTime.UtcNow;
        var hashtags = dto.Hashtags != null ? string.Join(",", dto.Hashtags) : null;
        var result = await _mediator.Send(new UpdateScheduleCommand(userId, id, dto.Content ?? string.Empty, scheduledFor, dto.ImageUrl, hashtags, dto.CallToAction));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result>> Delete(Guid id)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new DeleteScheduleCommand(userId, id));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{id:guid}/publish-now")]
    public async Task<ActionResult<Result<ScheduleDto>>> PublishNow(Guid id)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new PublishNowCommand(userId, id));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<Result<CalendarDto>>> GetCalendar([FromQuery] string month)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetCalendarQuery(userId, month));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
