using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class ProvisionFlatAdjustmentTests
{
    #region Variables

    private readonly Guid _matchedId = new("11111111-2222-3333-4444-555555555555");
    private readonly Guid _unmatchedId = new("66666666-7777-8888-9999-aaaaaaaaaaaa");

    #endregion

    #region Constructors

    public ProvisionFlatAdjustmentTests()
    {
    }

    #endregion

    #region ApplyAdjustment_NullFilter

    [Fact]
    public void ApplyAdjustment_NullFilter_AdjustsAllProvisions()
    {
        // Arrange
        var adjustment = new ProvisionFlatAdjustment(50f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(150f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_EmptyFilter

    [Fact]
    public void ApplyAdjustment_EmptyFilter_AdjustsAllProvisions()
    {
        // Arrange
        var adjustment = new ProvisionFlatAdjustment(50f, Array.Empty<Guid>());
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(150f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_MatchingFilter

    [Fact]
    public void ApplyAdjustment_MatchingFilter_AdjustsOnlyMatchingProvision()
    {
        // Arrange
        var filter = new[] { _matchedId };
        var adjustment = new ProvisionFlatAdjustment(50f, filter);
        var matched = new Provision(_matchedId, 100f);
        var unmatched = new Provision(_unmatchedId, 100f);

        // Act
        var matchedResult = adjustment.ApplyAdjustment(matched);
        var unmatchedResult = adjustment.ApplyAdjustment(unmatched);

        // Assert
        Assert.Equal(150f, matchedResult.Amount);
        Assert.Equal(100f, unmatchedResult.Amount);
    }

    #endregion

    #region ApplyAdjustment_NonMatchingFilter

    [Fact]
    public void ApplyAdjustment_NonMatchingFilter_LeavesProvisionUnchanged()
    {
        // Arrange
        var filter = new[] { _unmatchedId };
        var adjustment = new ProvisionFlatAdjustment(50f, filter);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(_matchedId, result.Id);
        Assert.Equal(100f, result.Amount);
    }

    #endregion

    #region ApplyAdjustment_NegativeAdjustment

    [Fact]
    public void ApplyAdjustment_NegativeAdjustment_NullFilter_ReducesAmountCorrectly()
    {
        // Arrange
        var adjustment = new ProvisionFlatAdjustment(-30f, null);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(70f, result.Amount);
    }

    [Fact]
    public void ApplyAdjustment_NegativeAdjustment_EmptyFilter_ReducesAmountCorrectly()
    {
        // Arrange
        var adjustment = new ProvisionFlatAdjustment(-30f, Array.Empty<Guid>());
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
        var adjustment = new ProvisionFlatAdjustment(-30f, filter);
        var provision = new Provision(_matchedId, 100f);

        // Act
        var result = adjustment.ApplyAdjustment(provision);

        // Assert
        Assert.Equal(70f, result.Amount);
    }

    #endregion
}
