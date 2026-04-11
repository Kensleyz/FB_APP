using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Billing.Commands;
using PageBoostAI.Application.Features.Billing.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("plans")]
    public async Task<ActionResult<Result<List<PlanDto>>>> GetPlans()
    {
        var result = await _mediator.Send(new GetPlansQuery());
        return Ok(result);
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<Result<string>>> Subscribe([FromBody] SubscribeDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new SubscribeCommand(userId, dto.Tier, dto.ReturnUrl, dto.CancelUrl));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("subscription")]
    public async Task<ActionResult<Result<SubscriptionDto>>> GetSubscription()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetSubscriptionQuery(userId));
        return Ok(result);
    }

    [HttpPost("subscription/cancel")]
    public async Task<ActionResult<Result>> CancelSubscription()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new CancelSubscriptionCommand(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("subscription/upgrade")]
    public async Task<ActionResult<Result<SubscriptionDto>>> Upgrade([FromBody] UpgradeDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new UpgradeSubscriptionCommand(userId, dto.NewTier));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("subscription/downgrade")]
    public async Task<ActionResult<Result<SubscriptionDto>>> Downgrade([FromBody] DowngradeDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new DowngradeSubscriptionCommand(userId, dto.NewTier));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("usage")]
    public async Task<ActionResult<Result<UsageDto>>> GetUsage()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetUsageQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<Result<List<TransactionDto>>>> GetTransactions()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetTransactionsQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
