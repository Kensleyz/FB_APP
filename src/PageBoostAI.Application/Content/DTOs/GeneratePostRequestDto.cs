namespace PageBoostAI.Application.Content.DTOs;

public record GeneratePostRequestDto(
    string BusinessType,
    string Tone,
    string PostType,
    string Language,
    string BusinessName,
    string BusinessDescription);
