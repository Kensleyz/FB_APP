using FluentAssertions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.ValueObjects;

public class PostContentTests
{
    [Fact]
    public void Create_WithValidContent_ShouldCreate()
    {
        // Act
        var content = new PostContent("Hello, this is a test post!");

        // Assert
        content.Text.Should().Be("Hello, this is a test post!");
    }

    [Fact]
    public void Create_WithEmptyContent_ShouldThrow()
    {
        // Act
        var act = () => new PostContent("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Post content cannot be empty*");
    }

    [Fact]
    public void Create_WithWhitespaceContent_ShouldThrow()
    {
        // Act
        var act = () => new PostContent("   ");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullContent_ShouldThrow()
    {
        // Act
        var act = () => new PostContent(null!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithContentOver280Chars_ShouldThrow()
    {
        // Arrange
        var longContent = new string('A', 281);

        // Act
        var act = () => new PostContent(longContent);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 280 characters*");
    }

    [Fact]
    public void Create_WithExactly280Chars_ShouldCreate()
    {
        // Arrange
        var exactContent = new string('A', 280);

        // Act
        var content = new PostContent(exactContent);

        // Assert
        content.Text.Should().HaveLength(280);
    }

    [Fact]
    public void Create_WithSingleCharacter_ShouldCreate()
    {
        // Act
        var content = new PostContent("A");

        // Assert
        content.Text.Should().Be("A");
    }

    [Fact]
    public void Equals_WithSameContent_ShouldReturnTrue()
    {
        // Arrange
        var content1 = new PostContent("Hello world");
        var content2 = new PostContent("Hello world");

        // Act & Assert
        content1.Equals(content2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentContent_ShouldReturnFalse()
    {
        // Arrange
        var content1 = new PostContent("Hello world");
        var content2 = new PostContent("Goodbye world");

        // Act & Assert
        content1.Equals(content2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameContent_ShouldBeEqual()
    {
        // Arrange
        var content1 = new PostContent("Test post");
        var content2 = new PostContent("Test post");

        // Act & Assert
        content1.GetHashCode().Should().Be(content2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnTextValue()
    {
        // Arrange
        var content = new PostContent("My post text");

        // Act & Assert
        content.ToString().Should().Be("My post text");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnStringValue()
    {
        // Arrange
        var content = new PostContent("Conversion test");

        // Act
        string result = content;

        // Assert
        result.Should().Be("Conversion test");
    }

    [Fact]
    public void MaxLength_ShouldBe280()
    {
        // Assert
        PostContent.MaxLength.Should().Be(280);
    }
}
