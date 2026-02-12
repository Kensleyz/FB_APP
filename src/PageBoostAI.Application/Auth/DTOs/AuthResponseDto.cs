namespace PageBoostAI.Application.Auth.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User);
