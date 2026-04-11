using System.Security.Claims;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Facebook.Commands;
using PageBoostAI.Application.Features.Facebook.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FacebookController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacebookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("connect")]
    public async Task<ActionResult<Result<string>>> Connect([FromBody] ConnectFacebookDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new ConnectFacebookCommand(userId, dto.RedirectUri));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<ActionResult<Result<List<FacebookPageDto>>>> Callback([FromQuery] string code, [FromQuery] string? state = null)
    {
        // State format: "{userId}|{frontendRedirectUri}"
        var parts = state?.Split('|', 2);
        if (parts is not { Length: 2 } || !Guid.TryParse(parts[0], out var userId))
            return BadRequest(Result<List<FacebookPageDto>>.Failure("Invalid state parameter."));

        var result = await _mediator.Send(new FacebookCallbackCommand(userId, code, state));
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("pages")]
    public async Task<ActionResult<Result<List<FacebookPageDto>>>> GetPages()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetUserPagesQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("pages/{pageId}/disconnect")]
    public async Task<ActionResult<Result>> Disconnect(Guid pageId)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new DisconnectPageCommand(userId, pageId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("pages/{pageId}/insights")]
    public async Task<ActionResult<Result<PageInsightsDto>>> GetInsights(Guid pageId)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetPageInsightsQuery(userId, pageId));
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
