using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Application.Auth.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    SubscriptionTier SubscriptionTier,
    bool IsEmailVerified,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
