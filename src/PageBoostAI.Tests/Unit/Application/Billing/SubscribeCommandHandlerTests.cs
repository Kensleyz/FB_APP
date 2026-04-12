using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Features.Billing.Commands;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Billing;

public class SubscribeCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ISubscriptionRepository _subscriptionRepository = Substitute.For<ISubscriptionRepository>();
    private readonly SubscribeCommandHandler _sut;

    private static readonly Guid UserId = Guid.NewGuid();

    public SubscribeCommandHandlerTests()
    {
        _sut = new SubscribeCommandHandler(_userRepository, _subscriptionRepository);
    }

    private User MakeUser()
    {
        var user = new User(new Email("user@example.com"), "hash", "John", "Doe");
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);
        return user;
    }

    [Theory]
    [InlineData("Starter")]
    [InlineData("Growth")]
    [InlineData("Pro")]
    public async Task Handle_ValidTier_CreatesSubscriptionAndReturnsId(string tier)
    {
        // Arrange
        MakeUser();
        _subscriptionRepository.GetActiveByUserIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns((Subscription?)null);
        _subscriptionRepository.AddAsync(Arg.Any<Subscription>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<Subscription>());

        var command = new SubscribeCommand(UserId, tier, null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Guid.TryParse(result.Data, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_FreeTier_ReturnsFailure()
    {
        // Arrange
        MakeUser();
        var command = new SubscribeCommand(UserId, "Free", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Free tier"));
    }

    [Fact]
    public async Task Handle_InvalidTier_ReturnsFailure()
    {
        // Arrange
        MakeUser();
        var command = new SubscribeCommand(UserId, "Diamond", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Invalid"));
    }

    [Fact]
    public async Task Handle_UserAlreadySubscribed_ReturnsFailure()
    {
        // Arrange
        var user = MakeUser();
        var existing = new Subscription(UserId, SubscriptionTier.Starter, Money.ZAR(99m), null);
        _subscriptionRepository.GetActiveByUserIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        var command = new SubscribeCommand(UserId, "Growth", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("already has an active subscription"));
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns((User?)null);
        var command = new SubscribeCommand(UserId, "Starter", null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("not found"));
    }

    [Fact]
    public async Task Handle_ValidSubscription_UpdatesUserTier()
    {
        // Arrange
        var user = MakeUser();
        _subscriptionRepository.GetActiveByUserIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns((Subscription?)null);
        _subscriptionRepository.AddAsync(Arg.Any<Subscription>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<Subscription>());

        var command = new SubscribeCommand(UserId, "Pro", null, null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        user.SubscriptionTier.Should().Be(SubscriptionTier.Pro);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }
}
