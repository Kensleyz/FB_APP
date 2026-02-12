using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Auth.Commands;
using PageBoostAI.Application.Features.Auth.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<Result<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
    {
        var result = await _mediator.Send(new RegisterCommand(dto.Email, dto.Password, dto.FirstName, dto.LastName, dto.PhoneNumber));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult<Result>> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var result = await _mediator.Send(new VerifyEmailCommand(dto.Token));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<Result<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var result = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));
        if (!result.IsSuccess) return Unauthorized(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<Result<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(dto.RefreshToken));
        if (!result.IsSuccess) return Unauthorized(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(dto.Email));
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult<Result>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(dto.Token, dto.NewPassword));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<Result<UserProfileDto>>> GetCurrentUser()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetCurrentUserQuery(userId));
        if (!result.IsSuccess) return NotFound(result);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<Result>> Logout()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new LogoutCommand(userId));
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
