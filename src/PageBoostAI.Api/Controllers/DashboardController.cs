using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Dashboard.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<Result<DashboardOverviewDto>>> GetOverview()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetDashboardOverviewQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<Result<AnalyticsDto>>> GetAnalytics([FromQuery] Guid pageId, [FromQuery] string period = "30d")
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetAnalyticsQuery(userId, pageId, period));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
