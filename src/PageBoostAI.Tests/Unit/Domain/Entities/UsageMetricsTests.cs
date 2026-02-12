using FluentAssertions;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Tests.Unit.Domain.Entities;

public class UsageMetricsTests
{
    private static readonly Guid ValidUserId = Guid.NewGuid();
    private const string ValidPeriod = "2026-02";

    private static UsageMetrics CreateValidMetrics() =>
        new(ValidUserId, ValidPeriod);

    [Fact]
    public void Create_WithValidData_ShouldCreateMetrics()
    {
        // Act
        var metrics = CreateValidMetrics();

        // Assert
        metrics.UserId.Should().Be(ValidUserId);
        metrics.Period.Should().Be(ValidPeriod);
        metrics.PostsGenerated.Should().Be(0);
        metrics.ImagesCreated.Should().Be(0);
        metrics.PostsPublished.Should().Be(0);
        metrics.ApiCallsCount.Should().Be(0);
    }

    [Fact]
    public void ForCurrentMonth_ShouldCreateWithCurrentPeriod()
    {
        // Act
        var metrics = UsageMetrics.ForCurrentMonth(ValidUserId);

        // Assert
        metrics.UserId.Should().Be(ValidUserId);
        metrics.Period.Should().Be(DateTime.UtcNow.ToString("yyyy-MM"));
    }

    [Fact]
    public void IncrementPosts_ShouldIncrementPostsGenerated()
    {
        // Arrange
        var metrics = CreateValidMetrics();

        // Act
        metrics.IncrementPosts();

        // Assert
        metrics.PostsGenerated.Should().Be(1);
        metrics.ApiCallsCount.Should().Be(1);
    }

    [Fact]
    public void IncrementPosts_MultipleTimes_ShouldAccumulate()
    {
        // Arrange
        var metrics = CreateValidMetrics();

        // Act
        metrics.IncrementPosts();
        metrics.IncrementPosts();
        metrics.IncrementPosts();

        // Assert
        metrics.PostsGenerated.Should().Be(3);
        metrics.ApiCallsCount.Should().Be(3);
    }

    [Fact]
    public void IncrementImages_ShouldIncrementImagesCreated()
    {
        // Arrange
        var metrics = CreateValidMetrics();

        // Act
        metrics.IncrementImages();

        // Assert
        metrics.ImagesCreated.Should().Be(1);
        metrics.ApiCallsCount.Should().Be(1);
    }

    [Fact]
    public void IncrementPublished_ShouldIncrementPostsPublished()
    {
        // Arrange
        var metrics = CreateValidMetrics();

        // Act
        metrics.IncrementPublished();

        // Assert
        metrics.PostsPublished.Should().Be(1);
    }

    [Fact]
    public void IncrementPublished_ShouldNotIncrementApiCallsCount()
    {
        // Arrange
        var metrics = CreateValidMetrics();

        // Act
        metrics.IncrementPublished();

        // Assert
        metrics.ApiCallsCount.Should().Be(0);
    }

    [Fact]
    public void HasReachedLimit_Free_WhenUnderLimit_ShouldReturnFalse()
    {
        // Arrange - Free tier limit is 5 posts
        var metrics = CreateValidMetrics();
        for (int i = 0; i < 4; i++) metrics.IncrementPosts();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Free);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasReachedLimit_Free_WhenAtLimit_ShouldReturnTrue()
    {
        // Arrange - Free tier limit is 5 posts
        var metrics = CreateValidMetrics();
        for (int i = 0; i < 5; i++) metrics.IncrementPosts();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Free);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedLimit_Starter_WhenAtLimit_ShouldReturnTrue()
    {
        // Arrange - Starter tier limit is 10 posts
        var metrics = CreateValidMetrics();
        for (int i = 0; i < 10; i++) metrics.IncrementPosts();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Starter);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedLimit_Growth_WhenAtLimit_ShouldReturnTrue()
    {
        // Arrange - Growth tier limit is 30 posts
        var metrics = CreateValidMetrics();
        for (int i = 0; i < 30; i++) metrics.IncrementPosts();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Growth);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedLimit_Pro_WhenAtLimit_ShouldReturnTrue()
    {
        // Arrange - Pro tier limit is 100 posts
        var metrics = CreateValidMetrics();
        for (int i = 0; i < 100; i++) metrics.IncrementPosts();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Pro);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedLimit_Free_WhenImagesAtLimit_ShouldReturnTrue()
    {
        // Arrange - Free tier image limit is 2
        var metrics = CreateValidMetrics();
        metrics.IncrementImages();
        metrics.IncrementImages();

        // Act
        var result = metrics.HasReachedLimit(SubscriptionTier.Free);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(SubscriptionTier.Free, 5, 1, 2)]
    [InlineData(SubscriptionTier.Starter, 10, 1, 5)]
    [InlineData(SubscriptionTier.Growth, 30, 3, 15)]
    [InlineData(SubscriptionTier.Pro, 100, 10, 50)]
    public void GetLimits_ShouldReturnCorrectLimitsForTier(
        SubscriptionTier tier, int expectedPosts, int expectedPages, int expectedImages)
    {
        // Act
        var (maxPosts, maxPages, maxImages) = UsageMetrics.GetLimits(tier);

        // Assert
        maxPosts.Should().Be(expectedPosts);
        maxPages.Should().Be(expectedPages);
        maxImages.Should().Be(expectedImages);
    }
}
