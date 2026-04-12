using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Features.Auth.Commands;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IJwtService _jwtService = Substitute.For<IJwtService>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
    {
        _sut = new RegisterCommandHandler(_userRepository, _jwtService, _emailService);

        _userRepository.ExistsByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _userRepository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<User>());
        _jwtService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("access-token");
        _jwtService.GenerateRefreshToken().Returns("refresh-token");
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new RegisterCommand("new@example.com", "Password123!", "Jane", "Smith", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.User.Email.Should().Be("new@example.com");
        result.Data.User.FirstName.Should().Be("Jane");
        result.Data.User.LastName.Should().Be("Smith");
        result.Data.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsFailure()
    {
        // Arrange
        _userRepository.ExistsByEmailAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var command = new RegisterCommand("existing@example.com", "Password123!", "John", "Doe", null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("already exists"));
    }

    [Fact]
    public async Task Handle_WithValidData_HashesPassword()
    {
        // Arrange
        var command = new RegisterCommand("new@example.com", "PlainPassword!", "Jane", "Smith", null);
        User? savedUser = null;

        _userRepository.AddAsync(Arg.Do<User>(u => savedUser = u), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<User>());

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.PasswordHash.Should().NotBe("PlainPassword!");
        BCrypt.Net.BCrypt.Verify("PlainPassword!", savedUser.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithPhoneNumber_IncludesPhoneInResponse()
    {
        // Arrange
        var command = new RegisterCommand("new@example.com", "Password123!", "Jane", "Smith", "+27821234567");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.User.PhoneNumber.Should().Be("+27821234567");
    }

    [Fact]
    public async Task Handle_WithValidData_PersistsUser()
    {
        // Arrange
        var command = new RegisterCommand("new@example.com", "Password123!", "Jane", "Smith", null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidData_SendsVerificationEmail()
    {
        // Arrange
        var command = new RegisterCommand("new@example.com", "Password123!", "Jane", "Smith", null);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendVerificationEmailAsync(
            "new@example.com",
            Arg.Is<string>(t => !string.IsNullOrEmpty(t)),
            Arg.Any<CancellationToken>());
    }
}
