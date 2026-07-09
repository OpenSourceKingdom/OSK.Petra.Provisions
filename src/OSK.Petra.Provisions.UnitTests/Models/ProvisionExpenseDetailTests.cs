using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class ProvisionExpenseDetailTests
{
    #region Variables

    private readonly Guid _testId = new("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    #endregion

    #region Constructors

    public ProvisionExpenseDetailTests()
    {
    }

    #endregion

    #region Constructor_FromProvidedAmountAndRequiredProvision

    [Fact]
    public void Constructor_FromProvidedAmountAndRequiredProvision_SetsCorrectValues()
    {
        // Arrange
        var required = new Provision(_testId, 100f);

        // Act
        var detail = new ProvisionExpenseDetail(80f, required);

        // Assert
        Assert.Equal(_testId, detail.Id);
        Assert.False(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(80f, detail.ProvidedAmount);
        Assert.Equal(20f, detail.OutstandingAmount);
    }

    #endregion

    #region Constructor_FromProvidedProvisionAndRequiredAmount

    [Fact]
    public void Constructor_FromProvidedProvisionAndRequiredAmount_SetsCorrectValues()
    {
        // Arrange
        var provided = new Provision(_testId, 120f);

        // Act
        var detail = new ProvisionExpenseDetail(provided, 100f);

        // Assert
        Assert.Equal(_testId, detail.Id);
        Assert.True(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(120f, detail.ProvidedAmount);
        Assert.Equal(0f, detail.OutstandingAmount);
    }

    #endregion

    #region Constructor_FromAllParams

    [Fact]
    public void Constructor_FromAllParams_SetsCorrectValues()
    {
        // Act
        var detail = new ProvisionExpenseDetail(_testId, 150f, 100f);

        // Assert
        Assert.Equal(_testId, detail.Id);
        Assert.Equal(150f, detail.RequiredAmount);
        Assert.Equal(100f, detail.ProvidedAmount);
    }

    #endregion

    #region Sufficient_ProvidedEqualsRequired

    [Fact]
    public void Sufficient_ProvidedEqualsRequired_ReturnsTrue()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 100f, 100f);

        // Assert
        Assert.True(detail.Sufficient);
    }

    #endregion

    #region Sufficient_ProvidedGreaterThanRequired

    [Fact]
    public void Sufficient_ProvidedGreaterThanRequired_ReturnsTrue()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 100f, 150f);

        // Assert
        Assert.True(detail.Sufficient);
    }

    #endregion

    #region Sufficient_ProvidedLessThanRequired

    [Fact]
    public void Sufficient_ProvidedLessThanRequired_ReturnsFalse()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 100f, 50f);

        // Assert
        Assert.False(detail.Sufficient);
    }

    #endregion

    #region OutstandingAmount_ZeroWhenSufficient

    [Fact]
    public void OutstandingAmount_ZeroWhenSufficient_ReturnsZero()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 100f, 150f);

        // Assert
        Assert.Equal(0f, detail.OutstandingAmount);
    }

    #endregion

    #region OutstandingAmount_PositiveWhenInsufficient

    [Theory]
    [InlineData(100f, 50f, 50f)]
    [InlineData(200f, 75f, 125f)]
    public void OutstandingAmount_PositiveWhenInsufficient_ReturnsDifference(float required, float provided, float expected)
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, required, provided);

        // Assert
        Assert.Equal(expected, detail.OutstandingAmount);
    }

    #endregion

    #region OutstandingAmount_ZeroWhenRequiredIsZero

    [Fact]
    public void OutstandingAmount_ZeroWhenRequiredIsZero_ReturnsZero()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 0f, 100f);

        // Assert
        Assert.Equal(0f, detail.OutstandingAmount);
    }

    #endregion

    #region OutstandingAmount_NegativeProvidedAmount

    [Fact]
    public void OutstandingAmount_NegativeProvidedAmount_ReturnsCorrectDifference()
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(_testId, 100f, -50f);

        // Assert
        Assert.Equal(150f, detail.OutstandingAmount);
        Assert.False(detail.Sufficient);
    }

    #endregion
}
