namespace PageBoostAI.Application.Content.DTOs;

public record GenerateImageRequestDto(string SearchQuery, string? OverlayText, bool OptimizeForFacebook = true);
