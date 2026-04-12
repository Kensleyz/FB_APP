using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Features.Auth.Commands;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Auth;

public class UpdateProfileCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateProfileCommandHandler _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static User MakeUser() => new(new Email("user@example.com"), "hash", "Old", "Name");

    public UpdateProfileCommandHandlerTests()
    {
        _sut = new UpdateProfileCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_WithValidUser_UpdatesProfileAndReturnsSuccess()
    {
        // Arrange
        var user = MakeUser();
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);

        var command = new UpdateProfileCommand(UserId, "New", "Name", "+27821234567");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FirstName.Should().Be("New");
        result.Data.LastName.Should().Be("Name");
        result.Data.PhoneNumber.Should().Be("+27821234567");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var command = new UpdateProfileCommand(UserId, "New", "Name", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("not found"));
    }

    [Fact]
    public async Task Handle_WithValidUser_PersistsChanges()
    {
        // Arrange
        var user = MakeUser();
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);

        var command = new UpdateProfileCommand(UserId, "Updated", "Person", null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ClearsPhoneNumber_WhenPassedNull()
    {
        // Arrange
        var user = MakeUser();
        _userRepository.GetByIdAsync(UserId, Arg.Any<CancellationToken>()).Returns(user);

        var command = new UpdateProfileCommand(UserId, "First", "Last", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Data!.PhoneNumber.Should().BeNull();
    }
}
