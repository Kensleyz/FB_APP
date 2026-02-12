namespace PageBoostAI.Application.Facebook.DTOs;

public record FacebookPageDto(
    Guid Id,
    string FacebookPageId,
    string PageName,
    string? PageCategory,
    string? ProfilePictureUrl,
    int FollowerCount,
    bool IsActive,
    DateTime ConnectedAt,
    DateTime? LastSyncedAt);
