using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Content.Commands.GenerateImage;
using PageBoostAI.Application.Content.Commands.GeneratePost;
using PageBoostAI.Application.Content.DTOs;
using PageBoostAI.Application.Content.Queries.GetTemplates;

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
    public async Task<ActionResult<Result<GeneratePostResponseDto>>> Generate([FromBody] GeneratePostRequestDto dto)
    {
        var result = await _mediator.Send(new GeneratePostCommand(
            dto.BusinessType, dto.Tone, dto.PostType, dto.Language, dto.BusinessName, dto.BusinessDescription));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("images/generate")]
    public async Task<ActionResult<Result<GenerateImageResponseDto>>> GenerateImage([FromBody] GenerateImageRequestDto dto)
    {
        var result = await _mediator.Send(new GenerateImageCommand(dto.SearchQuery, dto.OverlayText, dto.OptimizeForFacebook));
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("templates")]
    public async Task<ActionResult<Result<TemplatesDto>>> GetTemplates()
    {
        var result = await _mediator.Send(new GetTemplatesQuery());
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
