using Xunit;

namespace CalculatorLibrary.Tests.Unit;

public class CalculatorTests
{
    private readonly Calculator _sut = new();

    [Theory]
    [InlineData(5, 4, 9)]
    [InlineData(0, 0, 0)]
    [InlineData(-5, -5, -10)]
    [InlineData(5, 5, 10)]
    [InlineData(-5, 5, 0)]
    [InlineData(-15, -5, -20)]
    public void Add_ShouldAddTwoNumbers_WhenTwoNumbersAreIntegers(
        int a, int b, int expected)
    {
        // Act
        var result = _sut.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, -5, 0)]
    [InlineData(15, 5, 10)]
    [InlineData(-5, -5, 0)]
    [InlineData(-15, -5, -10)]
    [InlineData(5, -10, -5)]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTwoNumbersAreIntegers(
        int a, int b, int expected)
    {
        // Act
        var result = _sut.Subtract(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 5, 25)]
    [InlineData(50, 0, 0)]
    [InlineData(-5, 5, -25)]
    public void Multiply_ShouldMultiplyTwoNumbers_WhenTwoNumbersAreIntegers(
        int a, int b, int expected)
    {
        // Act
        var result = _sut.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 5, 1)]
    [InlineData(15, 5, 3)]
    [InlineData(-5, 1, -5, Skip = "Ignore mathematically impossible cases such as divide by zero being impossible")]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreIntegers(
        int a, int b, int expected)
    {
        // Act
        var result = _sut.Divide(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(Skip = "testing skip in fact")]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreIntegers2()
    {
        // Act
        var result = _sut.Divide(5, 5);

        // Assert
        Assert.Equal(1, result);
    }

    [Theory(Skip = "testing skip in theory")]
    [InlineData(5, 5, 1)]
    [InlineData(15, 5, 3)]
    [InlineData(-5, 0, 0, Skip = "Ignore mathematically impossible cases such as divide by zero being impossible")]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreIntegers3(
    int a, int b, int expected)
    {
        // Act
        var result = _sut.Divide(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}
