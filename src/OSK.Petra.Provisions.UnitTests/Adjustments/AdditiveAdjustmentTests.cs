using OSK.Petra.Provisions.Adjustments;
using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Adjustments;

public class AdditiveAdjustmentTests
{
    #region Variables

    private readonly Guid _matchedId = new("11111111-2222-3333-4444-555555555555");
    private readonly Guid _unmatchedId = new("66666666-7777-8888-9999-aaaaaaaaaaaa");

    #endregion

    #region ApplyAdjustment

    [Fact]
    public void ApplyAdjustment_NullFilter_AdjustsAllProvisions()
    {
        // Arrange
        var adjustment = new AdditiveAdjustment(50f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(150f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_EmptyFilter_AdjustsAllProvisions()
    {
        // Arrange
        var adjustment = new AdditiveAdjustment(50f, []);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(150f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_ProvisionFilterSet_AdjustsOnlyMatchingProvision()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new AdditiveAdjustment(50f, filter);
        var matched = new Provision(_matchedId, 100f);
        var unmatched = new Provision(_unmatchedId, 100f);

        // Act
        var matchedResult = adjustment.ApplyAdjustment(matched);
        var unmatchedResult = adjustment.ApplyAdjustment(unmatched);

        // Assert
        Assert.Equal(150f, matchedResult.Amount);
        Assert.Equal(100f, unmatchedResult.Amount);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApplyAdjustment_NegativeAdjustment_EmpyFilter_ReducesAmountCorrectly(bool useNull)
    {
        // Arrange
        var adjustment = new AdditiveAdjustment(-30f, useNull ? null : []);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(70f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_NegativeAdjustment_MatchingFilter_ReducesAmountCorrectly()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new AdditiveAdjustment(-30f, filter);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(70f, result.Amount);
    }

    #endregion
}
