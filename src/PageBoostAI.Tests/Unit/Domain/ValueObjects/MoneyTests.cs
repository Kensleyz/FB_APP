using FluentAssertions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Tests.Unit.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreate()
    {
        // Act
        var money = new Money(99.99m, "ZAR");

        // Assert
        money.Amount.Should().Be(99.99m);
        money.Currency.Should().Be("ZAR");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreate()
    {
        // Act
        var money = new Money(0m, "ZAR");

        // Assert
        money.Amount.Should().Be(0m);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrow()
    {
        // Act
        var act = () => new Money(-1m, "ZAR");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrow()
    {
        // Act
        var act = () => new Money(100m, "");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency is required*");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrencyToUpperCase()
    {
        // Act
        var money = new Money(100m, "zar");

        // Assert
        money.Currency.Should().Be("ZAR");
    }

    [Fact]
    public void ZAR_StaticFactory_ShouldCreateZARMoney()
    {
        // Act
        var money = Money.ZAR(150.00m);

        // Assert
        money.Amount.Should().Be(150.00m);
        money.Currency.Should().Be("ZAR");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAdd()
    {
        // Arrange
        var money1 = Money.ZAR(100m);
        var money2 = Money.ZAR(50m);

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("ZAR");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrow()
    {
        // Arrange
        var zar = Money.ZAR(100m);
        var usd = new Money(50m, "USD");

        // Act
        var act = () => zar.Add(usd);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtract()
    {
        // Arrange
        var money1 = Money.ZAR(100m);
        var money2 = Money.ZAR(30m);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Subtract_WithDifferentCurrency_ShouldThrow()
    {
        // Arrange
        var zar = Money.ZAR(100m);
        var usd = new Money(50m, "USD");

        // Act
        var act = () => zar.Subtract(usd);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.ZAR(99.99m);
        var money2 = Money.ZAR(99.99m);

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.ZAR(99.99m);
        var money2 = Money.ZAR(100.00m);

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "ZAR");
        var money2 = new Money(100m, "USD");

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
    }

    [Fact]
    public void CompareTo_ShouldCompareCorrectly()
    {
        // Arrange
        var smaller = Money.ZAR(50m);
        var larger = Money.ZAR(100m);

        // Act & Assert
        (smaller < larger).Should().BeTrue();
        (larger > smaller).Should().BeTrue();
        (smaller <= larger).Should().BeTrue();
        (larger >= smaller).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_WithDifferentCurrency_ShouldThrow()
    {
        // Arrange
        var zar = Money.ZAR(100m);
        var usd = new Money(50m, "USD");

        // Act
        var act = () => zar.CompareTo(usd);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.ZAR(99.99m);
        var money2 = Money.ZAR(99.99m);

        // Act & Assert
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = Money.ZAR(99.99m);

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Contain("ZAR");
        result.Should().Contain("99");
    }
}
