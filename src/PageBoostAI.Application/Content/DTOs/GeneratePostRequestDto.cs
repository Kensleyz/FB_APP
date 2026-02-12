using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Application.Content.DTOs;

public record GeneratePostRequestDto(
    BusinessType BusinessType,
    ToneOption Tone,
    PostType PostType,
    string Language,
    string BusinessName,
    string BusinessDescription);
