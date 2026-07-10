using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests;

public class ProvisionCalculatorTests
{
    #region Variables

    private readonly Guid _id1 = new("11111111-1111-1111-1111-111111111111");
    private readonly Guid _id2 = new("22222222-2222-2222-2222-222222222222");
    private readonly Guid _id3 = new("33333333-3333-3333-3333-333333333333");

    #endregion

    #region CalculateExpenditure

    [Fact]
    public void CalculateExpenditure_IEnumerable_ValidMatchingProvisions_ReturnsCorrectDetails()
    {
        // Arrange
        var required = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f) };
        var provided = new[] { new Provision(_id1, 80f), new Provision(_id2, 250f) };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(required, provided);

        // Assert
        Assert.Equal(2, result.Details.Count());
        var detail1 = result.Details.First(d => d.Id == _id1);
        var detail2 = result.Details.First(d => d.Id == _id2);
        Assert.False(detail1.Sufficient);
        Assert.True(detail2.Sufficient);
        Assert.Equal(20f, detail1.OutstandingAmount);
        Assert.Equal(0f, detail2.OutstandingAmount);
    }

    [Fact]
    public void CalculateExpenditure_IEnumerable_PartialMatch_ReturnsMatchingAndInsufficientDetails()
    {
        // Arrange
        var required = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f), new Provision(_id3, 50f) };
        var provided = new[] { new Provision(_id1, 100f) };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(required, provided);

        // Assert
        Assert.Equal(3, result.Details.Count());

        var matched = result.Details.First(d => d.Id == _id1);
        var unmatched1 = result.Details.First(d => d.Id == _id2);
        var unmatched2 = result.Details.First(d => d.Id == _id3);

        Assert.True(matched.Sufficient);
        Assert.False(unmatched1.Sufficient);
        Assert.Equal(200, unmatched1.RequiredAmount);
        Assert.False(unmatched2.Sufficient);
        Assert.Equal(50, unmatched2.RequiredAmount);
    }

    [Fact]
    public void CalculateExpenditure_IEnumerable_NoProvisionsRequired_ReturnsEmptySummary()
    {
        // Arrange
        var required = Array.Empty<Provision>();
        var provided = new[] { new Provision(_id1, 100f) };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(required, provided);

        // Assert
        Assert.True(result.Sufficient);
        Assert.Empty(result.Details);
    }

    [Fact]
    public void CalculateExpenditure_IEnumerable_NoProvisionsProvided_ReturnsExpectedSummary()
    {
        // Arrange
        var required = new[] { new Provision(_id1, 100f) };
        var provided = Array.Empty<Provision>();

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(required, provided);

        // Assert
        Assert.False(result.Sufficient);
        Assert.Single(result.Details);
    }

    [Fact]
    public void CalculateExpenditure_IEnumerable_MultipleProvisionsSameId_GroupsAndSumsCorrectly()
    {
        // Arrange
        var required = new[] { new Provision(_id1, 50f), new Provision(_id1, 50f) };
        var provided = new[] { new Provision(_id1, 80f), new Provision(_id1, 30f) };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(required, provided);

        // Assert
        var detail = result.Details.First();
        Assert.Equal(_id1, detail.Id);
        Assert.True(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(110f, detail.ProvidedAmount);
    }

    [Fact]
    public void CalculateExpenditure_Term_ValidProvisionsWithAdjustments_AppliesAdjustmentsToBothSides()
    {
        // Arrange
        var requiredTerm = new ProvisionTerm
        {
            Provisions = [new Provision(_id1, 100f)],
            Adjustments = [new AdditiveAdjustment(50f)]
        };
        var providedTerm = new ProvisionTerm
        {
            Provisions = [new Provision(_id1, 80f)],
            Adjustments = [new MultiplierAdjustment(2f)]
        };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(requiredTerm, providedTerm);

        // Assert
        var detail = result.Details.First();
        Assert.Equal(_id1, detail.Id);
        Assert.True(detail.Sufficient);
        Assert.Equal(150f, detail.RequiredAmount);
        Assert.Equal(160f, detail.ProvidedAmount);
    }

    [Fact]
    public void CalculateExpenditure_Term_ProvisionsWithNullAdjustments_ReturnsStandardDetails()
    {
        // Arrange
        var requiredTerm = new ProvisionTerm
        {
            Provisions = [new Provision(_id1, 100f)],
            Adjustments = null!
        };
        var providedTerm = new ProvisionTerm
        {
            Provisions = [new Provision(_id1, 120f)],
            Adjustments = null!
        };

        // Act
        var result = ProvisionCalculator.CalculateExpenditure(requiredTerm, providedTerm);

        // Assert
        var detail = result.Details.First();
        Assert.True(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(120f, detail.ProvidedAmount);
    }

    #endregion

    #region CalculateAdditiveAdjustedRecovery

    [Theory]
    [InlineData(100, 50)]
    [InlineData(100, -30)]
    [InlineData(100, 0)]
    public void CalculateAdditiveAdjustedRecovery_ValidProvisions_ReturnsAdjustedRecovery(float baseAmount, float adjustment)
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, baseAmount), new Provision(_id2, baseAmount) };

        // Act
        var result = ProvisionCalculator.CalculateAdditiveAdjustedRecovery(provisions, adjustment);

        // Assert
        var recovered = result.RecoveredProvisions.ToDictionary(p => p.Id);
        Assert.Equal(baseAmount + adjustment, recovered[_id1].Amount);
        Assert.Equal(baseAmount + adjustment, recovered[_id2].Amount);
    }

    [Fact]
    public void CalculateAdditiveAdjustedRecovery_EmptyProvisions_ReturnsEmptyRecovery()
    {
        // Arrange
        var provisions = Array.Empty<Provision>();

        // Act
        var result = ProvisionCalculator.CalculateAdditiveAdjustedRecovery(provisions, 50f);

        // Assert
        Assert.Empty(result.RecoveredProvisions);
    }

    #endregion

    #region CalculateMultiplierAdjustedRecovery

    [Theory]
    [InlineData(100f, 1.5f)]
    [InlineData(100f, 0.5f)]
    [InlineData(100f, -1f)]
    public void CalculateMultiplierAdjustedRecovery_ValidProvisions_ReturnsAdjustedRecovery(float baseAmount, float multiplier)
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, baseAmount), new Provision(_id2, baseAmount) };

        // Act
        var result = ProvisionCalculator.CalculateMultiplierAdjustedRecovery(provisions, multiplier);

        // Assert
        var recovered = result.RecoveredProvisions.ToDictionary(p => p.Id);
        Assert.Equal(baseAmount * multiplier, recovered[_id1].Amount);
        Assert.Equal(baseAmount * multiplier, recovered[_id2].Amount);
    }

    [Fact]
    public void CalculateMultiplierAdjustedRecovery_MultiplyOne_ReturnsOriginalAmounts()
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f) };

        // Act
        var result = ProvisionCalculator.CalculateMultiplierAdjustedRecovery(provisions, 1f);

        // Assert
        var recovered = result.RecoveredProvisions.ToDictionary(p => p.Id);
        Assert.Equal(100f, recovered[_id1].Amount);
        Assert.Equal(200f, recovered[_id2].Amount);
    }

    [Fact]
    public void CalculateMultiplierAdjustedRecovery_MultiplyZero_ReturnsZeroAmounts()
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, 100f) };

        // Act
        var result = ProvisionCalculator.CalculateMultiplierAdjustedRecovery(provisions, 0f);

        // Assert
        var recovered = result.RecoveredProvisions.First();
        Assert.Equal(0f, recovered.Amount);
    }

    #endregion

    #region CalculateRecovery

    [Fact]
    public void CalculateRecovery_NullProvisions_ReturnsEmptyRecovery()
    {
        // Act
        var result = ProvisionCalculator.CalculateRecovery(null!);

        // Assert
        Assert.Same(ProvisionRecoverySummary.Empty, result);
    }

    [Fact]
    public void CalculateRecovery_EmptyAdjustments_ReturnsUnchangedProvisions()
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f) };

        // Act
        var result = ProvisionCalculator.CalculateRecovery(provisions, Array.Empty<IProvisionAdjustment>());

        // Assert
        var recovered = result.RecoveredProvisions.ToDictionary(p => p.Id);
        Assert.Equal(100f, recovered[_id1].Amount);
        Assert.Equal(200f, recovered[_id2].Amount);
    }

    [Fact]
    public void CalculateRecovery_ValidProvisionsWithMultipleAdjustments_AppliesAllAdjustmentsSequentially()
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, 100f) };
        var adjustments = new IProvisionAdjustment[]
        {
            new AdditiveAdjustment(50f),
            new MultiplierAdjustment(0.5f)
        };

        // Act
        var result = ProvisionCalculator.CalculateRecovery(provisions, adjustments);

        // Assert
        var recovered = result.RecoveredProvisions.First();
        Assert.Equal(_id1, recovered.Id);
        Assert.Equal(75f, recovered.Amount);
    }

    [Fact]
    public void CalculateRecovery_ProvisionsWithDuplicateIds_GroupsAndSumsCorrectly()
    {
        // Arrange
        var provisions = new[]
        {
            new Provision(_id1, 30f),
            new Provision(_id1, 70f)
        };

        // Act
        var result = ProvisionCalculator.CalculateRecovery(provisions, Array.Empty<IProvisionAdjustment>());

        // Assert
        var recovered = result.RecoveredProvisions.First();
        Assert.Equal(_id1, recovered.Id);
        Assert.Equal(100f, recovered.Amount);
    }

    #endregion
}
