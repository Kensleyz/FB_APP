using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Webhooks.Commands;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("payfast")]
    public async Task<ActionResult> PayFastNotification([FromForm] PayFastNotificationDto dto)
    {
        var sourceIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _mediator.Send(new ProcessPayFastNotificationCommand(dto, sourceIp));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok();
    }
}
