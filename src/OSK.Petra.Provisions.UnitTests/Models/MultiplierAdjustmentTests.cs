using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class MultiplierAdjustmentTests
{
    #region Variables

    private readonly Guid _matchedId = new("11111111-2222-3333-4444-555555555555");
    private readonly Guid _unmatchedId = new("66666666-7777-8888-9999-aaaaaaaaaaaa");

    #endregion

    #region ApplyAdjustment_NullFilter

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApplyAdjustment_NullFilter_MultipliesAllProvisions(bool useNull)
    {
        // Arrange
        var adjustment = new MultiplierAdjustment(2f, useNull ? null : []);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(200f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_MatchingFilter_MultipliesOnlyMatchingProvision()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new MultiplierAdjustment(2f, filter);
        var matched = new Provision(_matchedId, 100f);
        var unmatched = new Provision(_unmatchedId, 100f);

        // Act
        var matchedResult = adjustment.ApplyAdjustment(matched);
        var unmatchedResult = adjustment.ApplyAdjustment(unmatched);

        // Assert
        Assert.Equal(200f, matchedResult.Amount);
        Assert.Equal(100f, unmatchedResult.Amount);
    }

    [Fact]
    public void ApplyAdjustment_MultiplierOne_ReturnsOriginalAmount()
    {
        // Arrange
        var adjustment = new MultiplierAdjustment(1f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(100f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_MultiplierZero_ReturnsZeroAmount()
    {
        // Arrange
        var adjustment = new MultiplierAdjustment(0f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(0f, result.Amount);
    }

    [Theory]
    [InlineData(-1f, 100f, -100f)]
    [InlineData(-0.5f, 200f, -100f)]
    public void ApplyAdjustment_NegativeMultiplier_ReturnsNegativeAmount(float percentage, float baseAmount, float expected)
    {
        // Arrange
        var adjustment = new MultiplierAdjustment(percentage, null);
        var provision = new Provision(_matchedId, baseAmount);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(expected, result.Amount);
    }

    #endregion
}
