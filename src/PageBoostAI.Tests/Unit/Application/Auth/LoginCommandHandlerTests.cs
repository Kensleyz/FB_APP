using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Features.Auth.Commands;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IJwtService _jwtService = Substitute.For<IJwtService>();
    private readonly LoginCommandHandler _sut;

    private static readonly Email TestEmail = new("user@example.com");
    private const string PlainPassword = "Password123!";

    public LoginCommandHandlerTests()
    {
        _sut = new LoginCommandHandler(_userRepository, _jwtService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword(PlainPassword);
        var user = new User(TestEmail, hash, "John", "Doe");

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("access-token");
        _jwtService.GenerateRefreshToken().Returns("refresh-token");

        var command = new LoginCommand("user@example.com", PlainPassword);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AccessToken.Should().Be("access-token");
        result.Data.RefreshToken.Should().Be("refresh-token");
        result.Data.User.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new LoginCommand("nobody@example.com", PlainPassword);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Invalid email or password"));
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword("correct-password");
        var user = new User(TestEmail, hash, "John", "Doe");

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(user);

        var command = new LoginCommand("user@example.com", "wrong-password");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Invalid email or password"));
    }

    [Fact]
    public async Task Handle_SuccessfulLogin_RecordsLastLoginAt()
    {
        // Arrange
        var hash = BCrypt.Net.BCrypt.HashPassword(PlainPassword);
        var user = new User(TestEmail, hash, "John", "Doe");
        var before = DateTime.UtcNow;

        _userRepository.GetByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("token");
        _jwtService.GenerateRefreshToken().Returns("refresh");

        var command = new LoginCommand("user@example.com", PlainPassword);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeOnOrAfter(before);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }
}
