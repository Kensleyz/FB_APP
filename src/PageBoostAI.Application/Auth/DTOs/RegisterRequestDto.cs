namespace PageBoostAI.Application.Auth.DTOs;

public record RegisterRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber);
