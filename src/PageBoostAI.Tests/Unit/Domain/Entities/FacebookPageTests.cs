using FluentAssertions;
using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Tests.Unit.Domain.Entities;

public class FacebookPageTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private const string ValidFacebookPageId = "fb_page_123";
    private const string ValidPageName = "My Business Page";
    private const string ValidAccessToken = "access_token_xyz";

    private static FacebookPage CreateValidPage() =>
        new(ValidUserId, ValidFacebookPageId, ValidPageName, ValidAccessToken);

    [Fact]
    public void Create_WithValidData_ShouldCreatePage()
    {
        // Act
        var page = CreateValidPage();

        // Assert
        page.UserId.Should().Be(ValidUserId);
        page.FacebookPageId.Should().Be(ValidFacebookPageId);
        page.PageName.Should().Be(ValidPageName);
        page.PageAccessToken.Should().Be(ValidAccessToken);
        page.IsActive.Should().BeTrue();
        page.FollowerCount.Should().Be(0);
        page.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldSetThem()
    {
        // Act
        var page = new FacebookPage(
            ValidUserId, ValidFacebookPageId, ValidPageName, ValidAccessToken,
            pageCategory: "Restaurant",
            profilePictureUrl: "https://example.com/pic.jpg");

        // Assert
        page.PageCategory.Should().Be("Restaurant");
        page.ProfilePictureUrl.Should().Be("https://example.com/pic.jpg");
    }

    [Fact]
    public void RefreshToken_ShouldUpdateTokenAndExpiry()
    {
        // Arrange
        var page = CreateValidPage();
        var newToken = "new_access_token";
        var expiresAt = DateTime.UtcNow.AddDays(60);

        // Act
        page.RefreshToken(newToken, expiresAt);

        // Assert
        page.PageAccessToken.Should().Be(newToken);
        page.AccessTokenExpiresAt.Should().Be(expiresAt);
        page.LastSyncedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var page = CreateValidPage();

        // Act
        page.Deactivate();

        // Assert
        page.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldStillSetFalse()
    {
        // Arrange
        var page = CreateValidPage();
        page.Deactivate();

        // Act - calling deactivate again (entity doesn't throw, just sets false again)
        page.Deactivate();

        // Assert
        page.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateFollowerCount_ShouldUpdateCount()
    {
        // Arrange
        var page = CreateValidPage();

        // Act
        page.UpdateFollowerCount(1500);

        // Assert
        page.FollowerCount.Should().Be(1500);
        page.LastSyncedAt.Should().NotBeNull();
    }
}
