using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Models;

public class ProvisionExpenseDetailTests
{
    #region Constructors

    [Fact]
    public void Constructor_FromProvidedAmountAndRequiredProvision_SetsCorrectValues()
    {
        // Arrange
        var required = new Provision(Guid.Empty, 100f);

        // Act
        var detail = new ProvisionExpenseDetail(80f, required);

        // Assert
        Assert.Equal(Guid.Empty, detail.Id);
        Assert.False(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(80f, detail.ProvidedAmount);
        Assert.Equal(20f, detail.OutstandingAmount);
    }

    [Fact]
    public void Constructor_FromProvidedProvisionAndRequiredAmount_SetsCorrectValues()
    {
        // Arrange
        var provided = new Provision(Guid.Empty, 120f);

        // Act
        var detail = new ProvisionExpenseDetail(provided, 100f);

        // Assert
        Assert.Equal(Guid.Empty, detail.Id);
        Assert.True(detail.Sufficient);
        Assert.Equal(100f, detail.RequiredAmount);
        Assert.Equal(120f, detail.ProvidedAmount);
        Assert.Equal(0f, detail.OutstandingAmount);
    }

    [Fact]
    public void Constructor_FromAllParams_SetsCorrectValues()
    {
        // Act
        var detail = new ProvisionExpenseDetail(Guid.Empty, 150f, 100f);

        // Assert
        Assert.Equal(Guid.Empty, detail.Id);
        Assert.Equal(150f, detail.RequiredAmount);
        Assert.Equal(100f, detail.ProvidedAmount);
    }

    #endregion

    #region Sufficient (Get)

    [Theory]
    [InlineData(100, 150)]
    [InlineData(100, 100)]
    [InlineData(150, 100)]
    [InlineData(150, 0)]
    [InlineData(0, 150)]
    public void Sufficient_VaryingProvidedAndRequiredAmounts_ReturnsExpectedValue(float required, float provided)
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(Guid.Empty, required, provided);

        // Assert
        Assert.Equal(provided >= required, detail.Sufficient);
    }

    #endregion

    #region OutstandingAmount (Get)

    [Theory]
    [InlineData(100, 150)]
    [InlineData(100, 100)]
    [InlineData(150, 100)]
    [InlineData(150, 0)]
    [InlineData(0, 150)]
    public void OutstandingAmount_ZeroWhenSufficient_ReturnsZero(float required, float provided)
    {
        // Arrange & Act
        var detail = new ProvisionExpenseDetail(Guid.Empty, required, provided);

        // Assert
        var expected = required - provided;
        expected = expected < 0
            ? 0 
            : expected;
        Assert.Equal(expected, detail.OutstandingAmount);
    }

    #endregion
}
