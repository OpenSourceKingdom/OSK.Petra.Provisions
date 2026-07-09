using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class ProvisionPercentageAdjustmentTests
{
    #region Variables

    private readonly Guid _matchedId = new("11111111-2222-3333-4444-555555555555");
    private readonly Guid _unmatchedId = new("66666666-7777-8888-9999-aaaaaaaaaaaa");

    #endregion

    #region Constructors

    public ProvisionPercentageAdjustmentTests()
    {
    }

    #endregion

    #region ApplyAdjustment_NullFilter

    [Fact]
    public void ApplyAdjustment_NullFilter_MultipliesAllProvisions()
    {
        // Arrange
        var adjustment = new ProvisionPercentageAdjustment(2f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(200f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_EmptyFilter

    [Fact]
    public void ApplyAdjustment_EmptyFilter_MultipliesAllProvisions()
    {
        // Arrange
        var adjustment = new ProvisionPercentageAdjustment(2f, Array.Empty<Guid>());
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(200f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_MatchingFilter

    [Fact]
    public void ApplyAdjustment_MatchingFilter_MultipliesOnlyMatchingProvision()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new ProvisionPercentageAdjustment(2f, filter);
        var matched = new Provision(_matchedId, 100f);
        var unmatched = new Provision(_unmatchedId, 100f);

        // Act
        var matchedResult = adjustment.ApplyAdjustment(matched);
        var unmatchedResult = adjustment.ApplyAdjustment(unmatched);

        // Assert
        Assert.Equal(200f, matchedResult.Amount);
        Assert.Equal(100f, unmatchedResult.Amount);
    }

    #endregion

    #region ApplyAdjustment_NonMatchingFilter

    [Fact]
    public void ApplyAdjustment_NonMatchingFilter_LeavesProvisionUnchanged()
    {
        // Arrange
        var filter = new[] { _unmatchedId };
        var adjustment = new ProvisionPercentageAdjustment(2f, filter);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(100f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_PercentageOnePointZero

    [Fact]
    public void ApplyAdjustment_PercentageOnePointZero_ReturnsOriginalAmount()
    {
        // Arrange
        var adjustment = new ProvisionPercentageAdjustment(1f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(100f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_PercentageZero

    [Fact]
    public void ApplyAdjustment_PercentageZero_ReturnsZeroAmount()
    {
        // Arrange
        var adjustment = new ProvisionPercentageAdjustment(0f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(0f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_NegativePercentage

    [Theory]
    [InlineData(-1f, 100f, -100f)]
    [InlineData(-0.5f, 200f, -100f)]
    public void ApplyAdjustment_NegativePercentage_ReturnsNegativeAmount(float percentage, float baseAmount, float expected)
    {
        // Arrange
        var adjustment = new ProvisionPercentageAdjustment(percentage, null);
        var provision = new Provision(_matchedId, baseAmount);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(expected, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_PartialFilter

    [Fact]
    public void ApplyAdjustment_PartialFilter_AdjustsOnlyIncludedIds()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new ProvisionPercentageAdjustment(1.5f, filter);
        var included = new Provision(_matchedId, 100f);
        var excluded = new Provision(_unmatchedId, 100f);

        // Act
        var includedResult = adjustment.ApplyAdjustment(included);
        var excludedResult = adjustment.ApplyAdjustment(excluded);

        // Assert
        Assert.Equal(150f, includedResult.Amount);
        Assert.Equal(100f, excludedResult.Amount);
    }

    #endregion
}
