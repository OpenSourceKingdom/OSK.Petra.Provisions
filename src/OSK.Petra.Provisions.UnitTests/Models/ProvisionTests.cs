using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class ProvisionTests
{
    #region Variables

    private readonly Guid _testId = new("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    #endregion

    #region Constructors

    [Fact]
    public void Constructor_WithIdOnly_SetsAmountToZero()
    {
        // Arrange & Act
        var provision = new Provision(_testId);

        // Assert
        Assert.Equal(_testId, provision.Id);
        Assert.Equal(0f, provision.Amount);
    }

    [Theory]
    [InlineData(100f)]
    [InlineData(-50f)]
    [InlineData(0f)]
    [InlineData(float.MaxValue / 2f)]
    public void Constructor_WithIdAndAmount_SetsCorrectValues(float amount)
    {
        // Arrange & Act
        var provision = new Provision(_testId, amount);

        // Assert
        Assert.Equal(_testId, provision.Id);
        Assert.Equal(amount, provision.Amount);
    }

    #endregion

    #region WithAmount

    [Fact]
    public void WithAmount_OriginalProvisionUnchanged_ReturnsDifferentInstance()
    {
        // Arrange
        var provision = new Provision(_testId, 50f);

        // Act
        var result = provision.WithAmount(75f);

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(50, provision.Amount);
        Assert.Equal(75, result.Amount);
    }

    #endregion

    #region Addition_ProvisionAndAmount

    [Theory]
    [InlineData(100f, 50f, 150f)]
    [InlineData(100f, -30f, 70f)]
    [InlineData(0f, 25f, 25f)]
    public void Addition_ProvisionAndAmount_ReturnsProvisionWithSummedAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount1);

        // Act
        var result = provision + amount2;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    [Theory]
    [InlineData(50f, 100f, 150f)]
    [InlineData(-30f, 100f, 70f)]
    [InlineData(25f, 0f, 25f)]
    public void Addition_AmountAndProvision_ReturnsProvisionWithSummedAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount2);

        // Act
        var result = amount1 + provision;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    #endregion

    #region Expend_ProvisionMinusAmount

    [Theory]
    [InlineData(100f, 30f, 70f)]
    [InlineData(100f, -20f, 120f)]
    [InlineData(50f, 50f, 0f)]
    public void Expend_ProvisionMinusAmount_ReturnsProvisionWithReducedAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount1);

        // Act
        var result = provision - amount2;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    [Theory]
    [InlineData(100f, 30f, 70f)]
    [InlineData(100f, -20, 120)]
    [InlineData(50f, 50f, 0f)]
    public void Expend_AmountMinusProvision_ReturnsProvisionWithReducedAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount2);

        // Act
        var result = amount1 - provision;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    #endregion

    #region Multiplication_ProvisionTimesAmount

    [Theory]
    [InlineData(10f, 3f, 30f)]
    [InlineData(10f, -2f, -20f)]
    [InlineData(10f, 0f, 0f)]
    public void Multiplication_ProvisionTimesAmount_ReturnsProvisionWithProductAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount1);

        // Act
        var result = provision * amount2;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    [Theory]
    [InlineData(3f, 10f, 30f)]
    [InlineData(-2f, 10f, -20f)]
    [InlineData(0f, 10f, 0f)]
    public void Multiplication_AmountTimesProvision_ReturnsProvisionWithProductAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount2);

        // Act
        var result = amount1 * provision;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    #endregion

    #region Division_ProvisionDividedByAmount

    [Theory]
    [InlineData(100f, 4f, 25f)]
    [InlineData(10f, -2f, -5f)]
    public void Division_ProvisionDividedByAmount_ReturnsProvisionWithQuotientAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount1);

        // Act
        var result = provision / amount2;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    [Fact]
    public void Division_ProvisionDividedByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        var provision = new Provision(_testId, 100f);

        // Act
        var infinity = provision / 0f;

        // Assert
        Assert.Equal(float.PositiveInfinity, infinity.Amount);
    }

    #endregion

    #region Division_AmountDividedByProvision

    [Theory]
    [InlineData(100f, 10f, 10f)]
    [InlineData(10f, -2f, -5f)]
    public void Division_AmountDividedByProvision_ReturnsProvisionWithQuotientAmount(float amount1, float amount2, float expected)
    {
        // Arrange
        var provision = new Provision(_testId, amount2);

        // Act
        var result = amount1 / provision;

        // Assert
        Assert.Equal(_testId, result.Id);
        Assert.Equal(expected, result.Amount);
    }

    [Fact]
    public void Division_AmountDividedByProvision_WithZeroAmount_ReturnsInifinity()
    {
        // Arrange
        var provision = new Provision(_testId, 0f);

        // Act
        var infinity = 100f / provision;

        // Assert
        Assert.Equal(float.PositiveInfinity, infinity.Amount);
    }

    #endregion
}
