using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Features.Billing.Queries;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Billing;

public class GetSubscriptionQueryHandlerTests
{
    private readonly ISubscriptionRepository _subscriptionRepository = Substitute.For<ISubscriptionRepository>();
    private readonly GetSubscriptionQueryHandler _sut;

    public GetSubscriptionQueryHandlerTests()
    {
        _sut = new GetSubscriptionQueryHandler(_subscriptionRepository);
    }

    [Fact]
    public async Task Handle_ActiveSubscriptionExists_ReturnsSubscriptionDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var subscription = new Subscription(userId, SubscriptionTier.Growth, Money.ZAR(249m), null);

        _subscriptionRepository.GetActiveByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(subscription);

        var query = new GetSubscriptionQuery(userId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Tier.Should().Be("Growth");
        result.Data.Amount.Should().Be(249m);
        result.Data.Currency.Should().Be("ZAR");
    }

    [Fact]
    public async Task Handle_NoActiveSubscription_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _subscriptionRepository.GetActiveByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Subscription?)null);

        var query = new GetSubscriptionQuery(userId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("No active subscription"));
    }
}
