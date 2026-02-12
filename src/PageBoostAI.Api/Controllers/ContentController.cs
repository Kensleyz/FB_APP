using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Application.Features.Content.Commands;
using PageBoostAI.Application.Features.Content.Queries;

namespace PageBoostAI.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<Result<GeneratedContentDto>>> Generate([FromBody] GenerateContentDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GenerateContentCommand(userId, dto.PageId, dto.BusinessType, dto.Tone, dto.PostType, dto.AdditionalContext));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("images/generate")]
    public async Task<ActionResult<Result<GeneratedImageDto>>> GenerateImage([FromBody] GenerateImageDto dto)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GenerateImageCommand(userId, dto.Prompt, dto.Style));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("templates")]
    public async Task<ActionResult<Result<List<ContentTemplateDto>>>> GetTemplates()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetContentTemplatesQuery(userId));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}
