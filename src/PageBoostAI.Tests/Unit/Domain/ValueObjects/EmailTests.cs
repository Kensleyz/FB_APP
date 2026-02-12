using FluentAssertions;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldCreate()
    {
        // Act
        var email = new Email("user@example.com");

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Theory]
    [InlineData("user@domain.co.za")]
    [InlineData("name.surname@company.com")]
    [InlineData("test+tag@gmail.com")]
    public void Create_WithVariousValidEmails_ShouldCreate(string validEmail)
    {
        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail.ToLowerInvariant());
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrow()
    {
        // Act
        var act = () => new Email("invalid-email");

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("plaintext")]
    public void Create_WithVariousInvalidEmails_ShouldThrow(string invalidEmail)
    {
        // Act
        var act = () => new Email(invalidEmail);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var act = () => new Email(null!);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void Create_ShouldNormalizeToLowerCase()
    {
        // Act
        var email = new Email("User@EXAMPLE.COM");

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("user1@example.com");
        var email2 = new Email("user2@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSameEmailDifferentCase_ShouldReturnTrue()
    {
        // Arrange - both should normalize to lowercase
        var email1 = new Email("User@Example.COM");
        var email2 = new Email("user@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameEmails_ShouldBeEqual()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act & Assert
        email.ToString().Should().Be("test@example.com");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnStringValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string result = email;

        // Assert
        result.Should().Be("test@example.com");
    }
}
