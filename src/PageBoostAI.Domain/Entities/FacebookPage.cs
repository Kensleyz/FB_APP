using PageBoostAI.Domain.Common;

namespace PageBoostAI.Domain.Entities;

public sealed class FacebookPage : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FacebookPageId { get; private set; } = string.Empty;
    public string PageName { get; private set; } = string.Empty;
    public string? PageCategory { get; private set; }
    public string PageAccessToken { get; private set; } = string.Empty;
    public DateTime? AccessTokenExpiresAt { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public int FollowerCount { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime ConnectedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastSyncedAt { get; private set; }

    private FacebookPage() { } // EF Core

    public FacebookPage(
        Guid userId,
        string facebookPageId,
        string pageName,
        string pageAccessToken,
        string? pageCategory = null,
        string? profilePictureUrl = null)
    {
        UserId = userId;
        FacebookPageId = facebookPageId;
        PageName = pageName;
        PageAccessToken = pageAccessToken;
        PageCategory = pageCategory;
        ProfilePictureUrl = profilePictureUrl;
    }

    public void RefreshToken(string newAccessToken, DateTime expiresAt)
    {
        PageAccessToken = newAccessToken;
        AccessTokenExpiresAt = expiresAt;
        LastSyncedAt = DateTime.UtcNow;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public void UpdateFollowerCount(int count)
    {
        FollowerCount = count;
        LastSyncedAt = DateTime.UtcNow;
        SetUpdated();
    }
}
