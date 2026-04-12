using FluentAssertions;
using NSubstitute;
using PageBoostAI.Application.Features.Auth.Queries;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Application.Auth;

public class GetCurrentUserQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetCurrentUserQueryHandler _sut;

    public GetCurrentUserQueryHandlerTests()
    {
        _sut = new GetCurrentUserQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsUserProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(new Email("user@example.com"), "hash", "John", "Doe", "+27821234567");

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var query = new GetCurrentUserQuery(userId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Email.Should().Be("user@example.com");
        result.Data.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
        result.Data.PhoneNumber.Should().Be("+27821234567");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var query = new GetCurrentUserQuery(userId);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("not found"));
    }
}
